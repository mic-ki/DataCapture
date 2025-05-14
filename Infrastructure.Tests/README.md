# Infrastructure.Tests

Tento projekt obsahuje unit testy pro ověření správného fungování Mediatoru a souvisejících komponent.

## Struktura testů

Testy jsou rozděleny do několika souborů:

- **MediatorTests.cs** - Základní testy pro Mediator třídu
- **ParallelNotificationPublisherTests.cs** - Testy pro ParallelNotificationPublisher
- **MediatorIntegrationTests.cs** - Integrační testy pro Mediator a DomainEventPublisher
- **CacheBehaviorTests.cs** - Testy pro CacheBehavior

## Spuštění testů

Testy lze spustit pomocí následujících příkazů:

```bash
# Přejděte do adresáře s testy
cd Infrastructure.Tests

# Spusťte testy
dotnet test
```

Nebo můžete spustit testy přímo z Visual Studio nebo Rider.

## Testované scénáře

1. **Základní funkce Mediatoru**
   - Správné volání handleru pro request
   - Správné pořadí volání pipeline behaviors
   - Správné volání všech handlerů pro notifikaci

2. **Paralelní publikování notifikací**
   - Všechny handlery jsou volány paralelně
   - Chyba v jednom handleru neovlivní ostatní

3. **Integrační testy**
   - Publikování doménových událostí přes DomainEventPublisher
   - Zpracování více doménových událostí

4. **Cache behavior**
   - Vrácení hodnoty z cache, pokud existuje
   - Volání handleru a uložení výsledku do cache, pokud hodnota v cache neexistuje
