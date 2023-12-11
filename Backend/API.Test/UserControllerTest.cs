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
            result.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Username is already registered.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
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
            result.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email address is already registered.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

    }
}