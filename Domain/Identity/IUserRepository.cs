namespace Domain.Identity;

// TODO
public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
}