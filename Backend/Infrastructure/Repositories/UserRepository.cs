using Entity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly InstaLiteContext _context;

    public UserRepository(InstaLiteContext context)
    {
        _context = context;
    }

    public async Task<User> AddUser(User user)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> GetUser(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public Task<List<User>> GetUsers()
    {
        return _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public Task<bool> VerifyPassword(string enteredPassword, string hashedPassword)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword));
    }

    public async Task<User> DeleteUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUser(User user)
    {

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }


}