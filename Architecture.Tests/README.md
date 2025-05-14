# Architecture.Tests

Tento projekt obsahuje testy pro ověření správných závislostí mezi vrstvami Clean Architecture.

## Účel projektu

Cílem tohoto projektu je zajistit, že závislosti mezi vrstvami architektury odpovídají principům Clean Architecture:

1. **Domain** - Nesmí být závislá na žádné jiné vrstvě
2. **Application** - Smí být závislá pouze na vrstvě Domain
3. **Infrastructure** - Smí být závislá na vrstvách Application a Domain
4. **Presentation (DataCapture)** - Smí být závislá na vrstvách Application a Infrastructure

## Struktura testů

Testy jsou implementovány pomocí knihovny NetArchTest.Rules a jsou rozděleny do několika kategorií:

- **Domain_Should_Not_DependOnOtherLayers** - Ověřuje, že vrstva Domain nemá závislosti na jiných vrstvách
- **Application_Should_DependOnDomain_Only** - Ověřuje, že vrstva Application má závislosti pouze na vrstvě Domain
- **Infrastructure_Should_Not_DependOnPresentation** - Ověřuje, že vrstva Infrastructure nemá závislosti na vrstvě Presentation
- **Presentation_Should_Not_HaveDependencyOnInfrastructureImplementations** - Ověřuje, že vrstva Presentation používá pouze abstrakce, nikoliv konkrétní implementace z Infrastructure

## Spuštění testů

Testy můžete spustit pomocí následujících příkazů:

```bash
# Přejděte do adresáře s testy
cd Architecture.Tests

# Spusťte testy
dotnet test
```

Nebo můžete spustit testy přímo z Visual Studio nebo Rider.

## Interpretace výsledků

Pokud některý z testů selže, znamená to, že byla porušena pravidla Clean Architecture. V takovém případě je potřeba upravit kód tak, aby odpovídal principům Clean Architecture.
