using Entity;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test.Repositories
{
    [TestClass]
    public class UserRepositoryTest
    {
        private DbContextOptions<InstaLiteContext> _options;
        private InstaLiteContext _context;
        private IUserRepository _userRepository;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<InstaLiteContext>()
                .UseInMemoryDatabase(databaseName: "InstaLite")
                .Options;

            _context = new InstaLiteContext(_options);
            _userRepository = new UserRepository(_context);
        }

        [TestMethod]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "Rick",
                PasswordHash = "1Password",
                Email = "ricky@test.com"
            };

            // Act
            var addedUser = await _userRepository.AddUser(user);

            // Assert
            addedUser.Should().NotBeNull();
            addedUser.Should().BeEquivalentTo(user);
        }

        [TestMethod]
        public async Task AddUser_ShouldEncryptPassword()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "ricky",
                PasswordHash = "TestPassword",
                Email = "rickTheD@test.com"
            };

            // Act
            var addedUser = await _userRepository.AddUser(user);

            // Assert
            addedUser.Should().NotBeNull(); 
            addedUser.PasswordHash.Should().NotBeNullOrEmpty();
            addedUser.PasswordHash.Should().NotBe("TestPassword"); 
            addedUser.PasswordHash.Should().NotContain("TestPassword");  
        }

        [TestMethod]
        public async Task GetUser_ShouldReturnUserById()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "John",
                PasswordHash = "hashedPassword",
                Email = "john@test.com"
            };

            await _userRepository.AddUser(user);

            // Act
            var retrievedUser = await _userRepository.GetUser(user.Id);

            // Assert
            retrievedUser.Should().NotBeNull();
            retrievedUser.Should().BeEquivalentTo(user);
        }

        [TestMethod]
        public async Task GetUsers_ShouldReturnListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
            new User { Id = 1, Username = "User1", PasswordHash = "Hash1", Email = "user1@test.com" },
            new User { Id = 2, Username = "User2", PasswordHash = "Hash2", Email = "user2@test.com" },
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(users.Count);

            result.Should().ContainEquivalentOf(users[0]);
            result.Should().ContainEquivalentOf(users[1]);

            result.First(u => u.Username == "User1").Should().BeEquivalentTo(users[0]);
            result.First(u => u.Username == "User2").Should().BeEquivalentTo(users[1]);
        }

        [TestMethod]
        public async Task GetUserByEmail_ShouldReturnUserWithEmail()
        {
            // Arrange
            var users = new List<User>
            {
            new User { Id = 1, Username = "User1", PasswordHash = "Hash1", Email = "user1@test.com" },
            new User { Id = 2, Username = "User2", PasswordHash = "Hash2", Email = "user2@test.com" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Act
            var userEmail = "user1@test.com";
            var result = await _userRepository.GetUserByEmail(userEmail);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(userEmail);

            result.Should().BeEquivalentTo(users[0]);
        }

        [TestMethod]
        public async Task GetUserByUsername_ShouldReturnUserWithUsername()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1", PasswordHash = "Hash1", Email = "user1@test.com" },
                new User { Id = 2, Username = "User2", PasswordHash = "Hash2", Email = "user2@test.com" },
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Act
            var username = "User1";
            var result = await _userRepository.GetUserByUsername(username);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be(username);

            result.Should().BeEquivalentTo(users[0]);
        }

        [TestMethod]
        public async Task VerifyPassword_ShouldReturnTrueForCorrectPassword()
        {
            // Arrange
            var password = "SecretPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Act
            var result = await _userRepository.VerifyPassword(password, hashedPassword);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task DeleteUserById_ShouldDeleteUserIfExists()
        {
            // Arrange
            var userIdToDelete = 1;
            var existingUser = new User {
                Id = userIdToDelete,
                Username = "ExistingUser",
                PasswordHash = "Hash1",
                Email = "existingUser@test.com"
            };

            using (var context = new InstaLiteContext(_options))
            {
                context.Users.Add(existingUser);
                context.SaveChanges();
            }

            // Act
            var deletedUser = await _userRepository.DeleteUserById(userIdToDelete);

            // Assert
            using (var context = new InstaLiteContext(_options))
            {
                var userInDatabase = await context.Users.FindAsync(userIdToDelete);

                deletedUser.Should().NotBeNull();
                deletedUser.Should().BeEquivalentTo(existingUser);
                userInDatabase.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task UpdateUser_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                Username = "ExistingUser",
                PasswordHash = "HashedPassword",
                Email = "existinguser@test.com"
            };

            using (var context = new InstaLiteContext(_options))
            {
                context.Users.Add(existingUser);
                context.SaveChanges();
            }

            var updatedUser = new User
            {
                Id = existingUser.Id,
                Username = "UpdatedUser",
                PasswordHash = "UpdatedPassword",
                Email = "updateduser@test.com"
            };

            // Act
            var result = await _userRepository.UpdateUser(updatedUser);

            // Assert
            using (var context = new InstaLiteContext(_options))
            {
                var userInDatabase = await context.Users.FindAsync(existingUser.Id);

                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(updatedUser);
                userInDatabase.Should().NotBeNull();
                userInDatabase.Should().BeEquivalentTo(updatedUser);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
