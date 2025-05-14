# Domain

Tento projekt obsahuje doménovou vrstvu aplikace podle principů Clean Architecture.

## Účel projektu

Doménová vrstva je jádrem aplikace a obsahuje:

1. **Byznys entity** - Objekty reprezentující klíčové koncepty domény
2. **Doménové události** - Události, které nastávají při změnách v doméně
3. **Základní rozhraní** - Definice základních rozhraní pro entity a služby

Doménová vrstva je nezávislá na ostatních vrstvách aplikace a neobsahuje žádné závislosti na infrastruktuře, uživatelském rozhraní nebo aplikační logice.

## Klíčové koncepty

### Entity

Entity jsou objekty s identitou, které reprezentují klíčové koncepty domény. Všechny entity dědí z abstraktních tříd:

- **BaseEntity<T>** - Základní třída pro všechny entity s identifikátorem typu T
- **BaseAuditableEntity<T>** - Rozšiřuje BaseEntity o auditovací informace (vytvoření, modifikace)
- **BaseSoftDeleteEntity<T>** - Rozšiřuje BaseEntity o podporu měkkého mazání
- **BaseAuditableSoftDeleteEntity<T>** - Kombinuje auditování a měkké mazání

### Doménové události

Doménové události reprezentují významné změny v doméně, které mohou vyžadovat reakci jiných částí systému:

- **DomainEvent** - Základní abstraktní třída pro všechny doménové události
- **Event<T, TEnum>** - Generická implementace doménové události

### Rozhraní

Doménová vrstva definuje základní rozhraní:

- **IEntity** - Marker rozhraní pro identifikaci entit
- **IEntity<T>** - Rozšířené rozhraní pro entity s identifikátorem
- **IAuditableEntity<T>** - Rozhraní pro entity s auditovacími informacemi
- **ISoftDelete** - Rozhraní pro entity podporující měkké mazání

## Struktura projektu

```
Domain/
├── Core/                  # Základní abstraktní třídy a rozhraní
│   ├── BaseEntity.cs
│   ├── BaseAuditableEntity.cs
│   ├── BaseSoftDeleteEntity.cs
│   ├── BaseAuditableSoftDeleteEntity.cs
│   ├── DomainEvent.cs
│   └── IEntity.cs
├── Identity/              # Entity související s identitou uživatelů
│   └── User.cs
└── SampleEntity.cs        # Ukázková entita
```

## Použití

### Vytvoření nové entity

```csharp
// Jednoduchá entita
public class Product : BaseEntity<int>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Entita s auditováním
public class Customer : BaseAuditableEntity<Guid>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Entita s měkkým mazáním
public class Order : BaseAuditableSoftDeleteEntity<int>
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
```

### Práce s doménovými událostmi

```csharp
// Vytvoření entity a přidání doménové události
var product = new Product { Name = "Nový produkt", Price = 100 };
product.AddDomainEvent(new Event<Product, EventType>(product, EventType.Created));
```

## Závislosti

Doménová vrstva neobsahuje žádné externí závislosti kromě standardních knihoven .NET. Toto je v souladu s principy Clean Architecture, kde doménová vrstva je nezávislá na ostatních vrstvách a technologiích.
