namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro notifikaci, která je publikována mediátorem.
/// Slouží jako marker interface pro identifikaci tříd, které představují notifikace.
/// Notifikace jsou zprávy, které mohou být zpracovány více handlery současně.
/// </summary>
public interface INotification
{

}