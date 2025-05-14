using Application.Models;

namespace Application.Abstraction;

public interface ISessionInfoService
{
    SessionInfo Session { get; }
    Task InitializeAsync();
}