using Application.Models;

namespace Application.Abstraction;
// TODO: Zjednodušit - možná úplně odebrat
public interface ICurrentUserAccessor
{
    SessionInfo? SessionInfo { get; }
}