using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Application.Services;
using Application.Services.Events;
using Application.Abstraction.Mediator;

namespace Infrastructure.Persistence;

/// <summary>
/// TODO:
/// Fabrika pro vytváření instancí ApplicationDbContext v době návrhu.
/// Používá se zatím jen na migrace. Asi bychom se bez ní mohli obejít.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Vytváří novou instanci ApplicationDbContext pro použití v době návrhu.
    /// </summary>
    /// <param name="args">Argumenty příkazové řádky</param>
    /// <returns>Nová instance ApplicationDbContext</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Načtení konfigurace z appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Použije aktuální adresář jako základní cestu
            .AddJsonFile("appsettings.json", optional: false)  // Načtení konfigurace z appsettings.json
            .Build();

        // Získání připojovacího řetězce z konfigurace
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Vytvoření možností pro DbContext
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(connectionString);  // Konfigurace pro použití SQLite databáze

        // Vytvoření mock implementací služeb potřebných pro ApplicationDbContext
        // V době návrhu nemáme přístup k DI kontejneru, proto musíme vytvořit vlastní implementace
        var currentUserService = new MockCurrentUserService();  // Mock služba pro aktuálního uživatele
        var domainEventPublisher = new MockDomainEventPublisher();  // Mock služba pro publikování událostí

        // Vytvoření a vrácení kontextu
        return new ApplicationDbContext(optionsBuilder.Options, currentUserService, domainEventPublisher);
    }
}

/// <summary>
/// Testovací implementace rozhraní ICurrentUserService pro vytváření kontextu v době návrhu.
/// Poskytuje fixní identifikátor uživatele "SYSTEM" pro auditovací účely.
/// </summary>
public class MockCurrentUserService : ICurrentUserService
{
    /// <summary>
    /// Vrací fixní identifikátor uživatele "SYSTEM" pro auditovací záznamy.
    /// </summary>
    public string? UserId => "SYSTEM";
}

/// <summary>
/// Testovací implementace třídy DomainEventPublisher pro vytváření kontextu v době návrhu.
/// Tato implementace neprovádí žádnou akci při publikování událostí, což je vhodné pro migrace a další nástroje.
/// </summary>
public class MockDomainEventPublisher : DomainEventPublisher
{
    /// <summary>
    /// Inicializuje novou instanci třídy MockDomainEventPublisher.
    /// Předává null jako mediátor, protože v době návrhu není potřeba.
    /// </summary>
    public MockDomainEventPublisher() : base(null)
    {
    }

    /// <summary>
    /// Poskytuje novou implementaci metody Publish, která neprovádí žádnou akci.
    /// Používáme klíčové slovo 'new' místo 'override', protože původní metoda není virtuální.
    /// </summary>
    /// <param name="domainEvent">Doménová událost k publikování</param>
    /// <returns>Dokončený Task</returns>
    public new Task Publish(Domain.Core.DomainEvent domainEvent)
    {
        // Neprovádíme žádnou akci v testovací implementaci
        return Task.CompletedTask;
    }
}
