using Api.Controllers;
using Api.Dtos;
using AutoMapper;
using AutoMapper.Internal;
using Entity;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Api.Test
{
    [TestClass]
    public class UserControllerTest
    {

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;

        private readonly UserController sut;

        public UserControllerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserController>>();

            sut = new UserController(_userRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetUsers_ReturnsOkResult_WithUserDtoList()
        {
            // Arrange
            var users = new List<User> { new User(), new User() };
            var userDtos = new List<UserDto> { new UserDto(), new UserDto() };

            _userRepositoryMock.Setup(repo => repo.GetUsers()).ReturnsAsync(users);
            _mapperMock.Setup(mapper => mapper.Map<List<UserDto>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await sut.GetUsers();

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(200);
            var returnValue = actionResult.Value as List<UserDto>;
            returnValue.Should().NotBeNull();
            returnValue.Should().Equal(userDtos);
        }

        [TestMethod]
        public async Task GetUserWithID_ReturnsOkResult_WithUserDto()
        {
            // Arrange
            var user = new User();
            var userDto = new UserDto();

            _userRepositoryMock.Setup(repo => repo.GetUser(It.IsAny<int>())).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

            // Act
            var result = await sut.GetUser(It.IsAny<int>());

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(200);
            var returnValue = actionResult.Value as UserDto;
            returnValue.Should().NotBeNull();
            returnValue.Should().Be(userDto);
        }

        [TestMethod]
        public async Task RegisterUser_WithUniqueUsernameAndEmail_ReturnsUserDto()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "testuser@test.com" };
            _userRepositoryMock.Setup(x => x.GetUserByEmail(user.Email)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(x => x.GetUserByUsername(user.Username)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(x => x.AddUser(user)).ReturnsAsync(user);

            // Act
            var result = await sut.RegisterUser(user);

            // Assert
            var actionResult = result.Result as CreatedAtActionResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(201);
            var userDto = actionResult.Value as UserDto;
            userDto.Should().NotBeNull();
            userDto.Username.Should().Be(user.Username);
            userDto.Email.Should().Be(user.Email);
        }

        [TestMethod]
        public async Task RegisterUser_WithDuplicateUsername_ShouldFail()
        {
            // Arrange
            var existingUser = new User { Username = "testuser", Email = "testuser@test.com" };
            var newUser = new User { Username = "testuser", Email = "newuser@test.com" };

            _userRepositoryMock.Setup(x => x.GetUserByUsername(newUser.Username)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(x => x.AddUser(newUser)).ReturnsAsync((User)null);

            // Act
            var result = await sut.RegisterUser(newUser);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(400);
            actionResult.Value.Should().BeEquivalentTo(new { error = "Username is already registered." });
        }

        [TestMethod]
        public async Task RegisterUser_WithDuplicateEmail_ShouldFail()
        {
            // Arrange
            var existingUser = new User { Username = "testuser", Email = "testuser@test.com" };
            var newUser = new User { Username = "testuser", Email = "newuser@test.com" };

            _userRepositoryMock.Setup(x => x.GetUserByEmail(newUser.Email)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(x => x.AddUser(newUser)).ReturnsAsync((User)null);

            // Act
            var result = await sut.RegisterUser(newUser);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(400);
            actionResult.Value.Should().BeEquivalentTo(new { error = "Email address is already registered." });
        }

        [TestMethod]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "validuser", Password = "validpassword" };
            var existingUser = new User { Username = loginModel.Username, PasswordHash = "hashedpassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginModel.Username)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(repo => repo.VerifyPassword(loginModel.Password, existingUser.PasswordHash)).ReturnsAsync(true);

            // Act
            var result = await sut.Login(loginModel);

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(200);
            actionResult.Value.Should().BeEquivalentTo(new { message = "Login successful." });
        }

        [TestMethod]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "invaliduser", Password = "invalidpassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginModel.Username)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.VerifyPassword(loginModel.Password, It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await sut.Login(loginModel);

            // Assert
            var actionResult = result.Result as UnauthorizedObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(401);
            actionResult.Value.Should().BeEquivalentTo(new { error = "Invalid username or password." });
        }
       
        [TestMethod]
        public async Task DeleteUser_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "testuser@test.com" };
            var userDto = new UserDto { Id = 1, Username = "testuser", Email = "testuser@test.com" };

            _userRepositoryMock.Setup(repo => repo.DeleteUserById(user.Id)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

            // Act
            var result = await sut.DeleteUser(user.Id);

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(200);

            userDto.Should().NotBeNull();
            userDto.Id.Should().Be(user.Id);
        }

        [TestMethod]
        public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidUserId = 456;
            _userRepositoryMock.Setup(repo => repo.DeleteUserById(invalidUserId)).ReturnsAsync((User)null);

            // Act
            var result = await sut.DeleteUser(invalidUserId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task UpdateUser_WithValidId_ReturnsOkResult_AndUpdatesUser()
        {
            // Arrange
            var id = 1;
            var updateUserModel = new UpdateUserModel { Email = "test@test.com", Username = "testUser" };
            var user = new User { Id = id, Email = "old@test.com", Username = "oldUser" };
            var updatedUser = new User { Id = id, Email = updateUserModel.Email, Username = updateUserModel.Username };
            var userDto = new UserDto { Id = id, Email = updateUserModel.Email, Username = updateUserModel.Username };

            _userRepositoryMock.Setup(x => x.GetUser(id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.GetUserByEmail(updateUserModel.Email)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(x => x.GetUserByUsername(updateUserModel.Username)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(x => x.UpdateUser(It.IsAny<User>())).ReturnsAsync(updatedUser);
            _mapperMock.Setup(x => x.Map<UpdateUserModel, User>(updateUserModel, user)).Returns(updatedUser);
            _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

            var controller = new UserController(_userRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateUser(id, updateUserModel);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<UserDto>().Subject;
            returnValue.Should().BeEquivalentTo(userDto);
        }

        [TestMethod]
        public async Task UpdateUser_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var updateModel = new UpdateUserModel { Username = "updateduser", Email = "updateduser@test.com" };

            _userRepositoryMock.Setup(repo => repo.GetUser(userId)).ReturnsAsync((User)null);

            // Act
            var result = await sut.UpdateUser(userId, updateModel);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be(404);
            var returnValue = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject.Value;
            returnValue.Should().Be($"User with Id {userId} not found.");
        }

        [TestMethod]
        public async Task UpdateUser_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var updateModel = new UpdateUserModel { Username = "updateduser", Email = "updateduser@test.com" };
            var existingUser = new User { Id = userId, Username = "existinguser", Email = "existinguser@test.com" };
            var existingEmailUser = new User { Id = 2, Username = "otheruser", Email = updateModel.Email };

            _userRepositoryMock.Setup(repo => repo.GetUser(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(updateModel.Email)).ReturnsAsync(existingEmailUser);

            // Act
            var result = await sut.UpdateUser(userId, updateModel);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
            var returnValue = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject.Value;
            returnValue.Should().BeEquivalentTo(new { error = "Email address is already registered." });
        }

        [TestMethod]
        public async Task UpdateUser_WithDuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var updateModel = new UpdateUserModel { Username = "updateduser", Email = "updateduser@test.com" };
            var existingUser = new User { Id = userId, Username = "existinguser", Email = "existinguser@test.com" };
            var existingUsernameUser = new User { Id = 2, Username = updateModel.Username, Email = "otheruser@test.com" };

            _userRepositoryMock.Setup(repo => repo.GetUser(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(updateModel.Email)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(updateModel.Username)).ReturnsAsync(existingUsernameUser);

            // Act
            var result = await sut.UpdateUser(userId, updateModel);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
            var returnValue = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject.Value;
            returnValue.Should().BeEquivalentTo(new { error = "Username is already registered." });
        }

    }
}