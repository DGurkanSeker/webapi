using WebApi.Data;

public interface IUser
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
    Task<List<User>> GetUserByDateAsync(DateTime date);
   Task<bool> VerifyUserAsync(string email, string password);
}