namespace Domain.Identity;

// Domain Layer
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; } // Pro ukázku, v praxi použijte bezpečnější přístup
    public IReadOnlyCollection<string> Roles { get; private set; }

    // Doménové metody (např. změna hesla, přiřazení role)
    public void AssignRole(string role)
    {
        // ... obchodní pravidla ...
    }
}