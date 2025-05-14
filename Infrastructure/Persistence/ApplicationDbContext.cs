using System.Linq.Expressions;
using Application.Abstraction;
using Application.Services;
using Application.Services.Events;
using Domain;
using Domain.Core;
using Domain.System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Hlavní databázový kontext aplikace, který implementuje rozhraní IApplicationDbContext.
/// Rozšiřuje IdentityDbContext pro podporu uživatelských účtů a rolí.
/// Zajišťuje přístup k databázi a implementuje business logiku pro ukládání dat,
/// včetně auditování, soft delete a publikování doménových událostí.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    /// <summary>
    /// Služba pro získání informací o aktuálním uživateli.
    /// Používá se pro automatické vyplnění auditovacích polí.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Služba pro publikování doménových událostí.
    /// Používá se pro odesílání událostí po úspěšném uložení změn.
    /// </summary>
    private readonly DomainEventPublisher _domainEventPublisher;

    /// <summary>
    /// Inicializuje novou instanci třídy ApplicationDbContext.
    /// </summary>
    /// <param name="options">Konfigurace databázového kontextu</param>
    /// <param name="currentUserService">Služba pro získání informací o aktuálním uživateli</param>
    /// <param name="domainEventPublisher">Služba pro publikování doménových událostí</param>
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        DomainEventPublisher domainEventPublisher)
        : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventPublisher = domainEventPublisher;
    }

    /// <summary>
    /// Kolekce záznamů o auditování změn v databázi.
    /// </summary>
    public DbSet<AuditTrail> AuditTrails { get; set; }

    /// <summary>
    /// Kolekce systémových logů aplikace.
    /// </summary>
    public DbSet<SystemLog> SystemLogs { get; set; }

    /// <summary>
    /// Kolekce ukázkových entit pro demonstraci funkcionality.
    /// </summary>
    public DbSet<SampleEntity> SampleEntity { get; set; }

    /// <summary>
    /// Konfiguruje model databáze při jeho vytváření.
    /// </summary>
    /// <param name="modelBuilder">Builder pro konfiguraci modelu databáze</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Nejprve zavoláme základní implementaci, která nakonfiguruje Identity tabulky
        base.OnModelCreating(modelBuilder);

        // Aplikujeme všechny konfigurace entit z namespace Infrastructure.Persistence.Configurations
        // Toto automaticky najde a použije všechny třídy implementující IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Aplikace globálního filtru pro všechny entity implementující ISoftDelete
        // Tento filtr automaticky vyloučí z výsledků dotazů všechny entity, které byly označeny jako smazané
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                // Vytvoříme dynamický LINQ výraz pomocí Expression API
                var parameter = Expression.Parameter(entityType.ClrType, "e");                      // Parametr 'e' reprezentující entitu
                var property = Expression.Property(parameter, nameof(ISoftDelete.Deleted));         // Přístup k vlastnosti Deleted
                var nullCheck = Expression.Equal(property, Expression.Constant(null));              // Kontrola, zda je Deleted == null
                var lambda = Expression.Lambda(nullCheck, parameter);                               // Vytvoření lambda výrazu

                // Aplikace filtru na entitu - vrátí pouze záznamy, kde Deleted == null (tzn. nebyly smazány)
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Přepisuje standardní metodu SaveChangesAsync pro přidání vlastní business logiky
    /// při ukládání změn do databáze.
    /// </summary>
    /// <param name="cancellationToken">Token pro zrušení operace</param>
    /// <returns>Počet záznamů, které byly uloženy do databáze</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Získání aktuálního uživatele a času pro auditovací účely
        var currentUser = _currentUserService.UserId;
        var now = DateTime.UtcNow;

        // Automatické vyplnění auditovacích polí pro entity implementující IAuditableEntity
        // Toto zajišťuje, že všechny entity budou mít správně vyplněné časové značky a informace o uživateli
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity<object>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Pro nově přidané entity nastavujeme čas vytvoření a uživatele, který je vytvořil
                    entry.Entity.Created = now;
                    entry.Entity.CreatedBy = currentUser;
                    break;
                case EntityState.Modified:
                    // Pro modifikované entity nastavujeme čas poslední modifikace a uživatele, který je modifikoval
                    entry.Entity.LastModified = now;
                    entry.Entity.LastModifiedBy = currentUser;
                    break;
            }
        }

        // Implementace soft delete pro entity implementující ISoftDelete
        // Místo fyzického smazání záznamů z databáze pouze označíme záznam jako smazaný
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // Změníme stav entity z Deleted na Modified, aby nedošlo k fyzickému smazání
                entry.State = EntityState.Modified;
                // Nastavíme čas smazání a uživatele, který entitu smazal
                entry.Entity.Deleted = now;
                entry.Entity.DeletedBy = currentUser;
            }
        }

        // Získání doménových událostí před uložením změn
        // Tyto události budou publikovány až po úspěšném uložení změn do databáze
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity<object>>()
            .Where(e => e.Entity.DomainEvents.Any())  // Vybereme pouze entity, které mají nějaké doménové události
            .Select(e => e.Entity)
            .ToList();

        // Získáme všechny doménové události ze všech entit
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Vyčistíme doménové události z entit, aby nebyly publikovány vícekrát
        entitiesWithEvents.ForEach(entity => entity.ClearDomainEvents());

        // Uložíme změny do databáze pomocí základní implementace SaveChangesAsync
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publikování doménových událostí po úspěšném uložení změn
        // Toto umožňuje dalším částem aplikace reagovat na změny v databázi
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventPublisher.Publish(domainEvent);
        }

        return result;
    }
}
