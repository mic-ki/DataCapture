using System.Security.Claims;
using Application.Abstraction;
using Application.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class SessionInfoService : ISessionInfoService
{
    public SessionInfo SessionInfo { get; private set; } = new SessionInfo();
    private bool _isPopulated = false;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public SessionInfoService(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public SessionInfo Session { get; }

    public async Task InitializeAsync()
    {
        // Zabráníme vícenásobné inicializaci, pokud již byla provedena a nedošlo k resetu
        if (_isPopulated) return;

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        SessionInfo.SessionStartTime = DateTime.UtcNow;
        SessionInfo.IpAddress = GetUserIpAddress();

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            SessionInfo.IsAuthenticated = true;
            SessionInfo.UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            SessionInfo.UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? user.Identity.Name;
            SessionInfo.Email = user.FindFirst(ClaimTypes.Email)?.Value;
            SessionInfo.Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            SessionInfo.Status = "Active"; // Příklad, toto si přizpůsobte
        }
        else
        {
            SessionInfo.IsAuthenticated = false;
            SessionInfo.UserName = "Anonymous";
            SessionInfo.Status = "Guest";
        }
        _isPopulated = true;
    }

    private string? GetUserIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;
        
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    public void Reset()
    {
        // Vytvoří novou instanci pro vymazání starých dat a resetuje příznak
        SessionInfo = new SessionInfo();
        _isPopulated = false;
    }

    // Metoda pro aktualizaci některých částí SessionInfo během session, pokud je to potřeba
    public void UpdateUserStatus(string newStatus)
    {
        SessionInfo.Status = newStatus;
        // Zde lze případně vyvolat událost, pokud jiné části UI potřebují reagovat
    }
}