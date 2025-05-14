using Application.Abstraction;

namespace Infrastructure;

// Infrastructure Layer
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Můžete přidat další vlastnosti specifické pro infrastrukturu
    public string? CustomInfrastructureProperty { get; set; }
}