using Domain.System;
using Infrastructure.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Konfigurační třída pro entitu AuditTrail.
/// Definuje mapování mezi objektovým modelem a databázovým schématem.
/// </summary>
public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    /// <summary>
    /// Konfiguruje mapování entity AuditTrail do databáze.
    /// </summary>
    /// <param name="builder">Builder pro konfiguraci entity</param>
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        // Použití vlastní konverze pro kolekce a slovníky, které jsou uloženy jako JSON řetězce v databázi
        // Toto umožňuje ukládat komplexní objekty do jednoho sloupce v databázi
        builder.Property(e => e.AffectedColumns).HasJsonConversion(); // Seznam ovlivněných sloupců
        builder.Property(u => u.OldValues).HasJsonConversion();       // Původní hodnoty před změnou
        builder.Property(u => u.NewValues).HasJsonConversion();       // Nové hodnoty po změně
        builder.Property(u => u.PrimaryKey)!.HasJsonConversion();     // Primární klíč auditované entity

        // Nastavení maximální délky pro textové sloupce obsahující detailní informace
        // int.MaxValue zajišťuje, že se do sloupce vejde libovolně dlouhý text
        builder.Property(x => x.DebugView).HasMaxLength(int.MaxValue);   // Ladící pohled na změny
        builder.Property(x => x.ErrorMessage).HasMaxLength(int.MaxValue); // Případná chybová zpráva
    }
}