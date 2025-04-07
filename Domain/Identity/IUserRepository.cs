namespace Domain.Identity;

// Domain Layer
public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
}