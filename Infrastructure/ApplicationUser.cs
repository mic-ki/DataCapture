using Application.Abstraction;

namespace Infrastructure;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? CustomInfrastructureProperty { get; set; }
}