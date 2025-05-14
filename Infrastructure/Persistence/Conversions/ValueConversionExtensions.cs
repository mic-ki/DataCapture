using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Conversions;

/// <summary>
/// Statická třída poskytující rozšiřující metody pro konverzi hodnot v EF.
/// Hlavní funkcí je konverze komplexních objektů na JSON řetězce pro uložení v databázi.
/// </summary>
public static class ValueConversionExtensions
{
    /// <summary>
    /// Rozšiřující metoda, která konfiguruje vlastnost entity pro konverzi mezi komplexním objektem a JSON řetězcem.
    /// Umožňuje ukládat komplexní objekty (např. slovníky, seznamy) jako JSON řetězce v databázi.
    /// </summary>
    /// <typeparam name="T">Typ vlastnosti, který bude konvertován na JSON</typeparam>
    /// <param name="propertyBuilder">Builder vlastnosti entity</param>
    /// <returns>Builder vlastnosti entity pro podporu řetězového volání metod</returns>
    public static PropertyBuilder<T?> HasJsonConversion<T>(this PropertyBuilder<T?> propertyBuilder)
    {
        // Získání standardních možností pro serializaci JSON
        var options = DefaultJsonSerializerOptions.Options;

        // Vytvoření konvertoru, který převádí mezi typem T a řetězcem JSON
        var converter = new ValueConverter<T?, string>(
            // Konverze z T na string (serializace)
            v => JsonSerializer.Serialize(v, options),
            // Konverze ze string na T (deserializace) s ošetřením null hodnot
            v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<T>(v, options));

        // Vytvoření porovnávače pro správné porovnávání hodnot v paměti
        var comparer = new ValueComparer<T?>(
            // Funkce pro porovnání dvou instancí - porovnává jejich JSON reprezentace
            (l, r) => JsonSerializer.Serialize(l, options) == JsonSerializer.Serialize(r, options),
            // Funkce pro výpočet hash kódu - používá hash kód JSON reprezentace
            v => v == null ? 0 : JsonSerializer.Serialize(v, options).GetHashCode(),
            // Funkce pro vytvoření kopie hodnoty - serializuje a následně deserializuje objekt
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, options), options));

        // Nastavení konvertoru pro vlastnost
        propertyBuilder.HasConversion(converter);
        // Nastavení porovnávače pro správné sledování změn v ChangeTrackeru
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}

/// <summary>
/// Třída poskytující standardní konfiguraci pro serializaci a deserializaci JSON.
/// Definuje jednotné nastavení pro všechny JSON operace v aplikaci.
/// </summary>
public class DefaultJsonSerializerOptions
{
    /// <summary>
    /// Standardní konfigurace pro JSON serializaci a deserializaci.
    /// </summary>
    /// <remarks>
    /// Konfigurace zahrnuje:
    /// - Použití camelCase pro názvy vlastností
    /// - Case-insensitive deserializaci
    /// - Podporu pro serializaci výčtových typů jako řetězců
    /// - Podporu pro základní latinku a čínské znaky v kódování
    /// </remarks>
    public static JsonSerializerOptions Options => new()
    {
        // Nastavení encoderu pro podporu základní latinky a čínských znaků
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs),
        // Použití camelCase pro názvy vlastností (např. FirstName -> firstName)
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Ignorování velikosti písmen při deserializaci
        PropertyNameCaseInsensitive = true,
        // Přidání konvertoru pro výčtové typy, aby byly serializovány jako řetězce
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}