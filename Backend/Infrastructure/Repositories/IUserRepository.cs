using Entity;

namespace Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User> DeleteUserById(int id);
        Task<User> GetUser(int id);
        Task<List<User>> GetUsers();
        Task<User> UpdateUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<bool> VerifyPassword(string enteredPassword, string hashedPassword);
    }
}
