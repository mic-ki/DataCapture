using Application.Abstraction;
using Application.Models;

namespace Infrastructure.Services.Identity;

public class CurrentUserAccessor : ICurrentUserAccessor
{

    public SessionInfo? SessionInfo { get; set; }
}