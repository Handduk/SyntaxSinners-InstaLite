using Api.Dtos;
using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Runtime.CompilerServices;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserRepository userRepository, 
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisterUser(User user)
        {
            try
            {
                var existingEmailUser = await _userRepository.GetUserByEmail(user.Email);
                if (existingEmailUser != null)
                {
                    _logger.LogError("Email address is already registered.");
                    return BadRequest(new { error = "Email address is already registered." });
                }

                var existingUsernameUser = await _userRepository.GetUserByUsername(user.Username);
                if (existingUsernameUser != null)
                {
                    _logger.LogError("Username is already registered.");
                    return BadRequest(new { error = "Username is already registered." });
                }

                var createdUser = await _userRepository.AddUser(user);
                var userDto = new UserDto { Id = createdUser.Id, Username = createdUser.Username, Email = createdUser.Email };
                return CreatedAtAction("GetUser", new { id = userDto.Id }, userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to register user." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginModel loginModel)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByUsername(loginModel.Username);

                if (existingUser != null && await _userRepository.VerifyPassword(loginModel.Password, existingUser.PasswordHash))
                {
                    return Ok(new { message = "Login successful." });
                }

                return Unauthorized(new { error = "Invalid username or password." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to login." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDto>> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.DeleteUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user.");
                return BadRequest(new { error = "Failed to delete user." });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserModel updateModel)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(id);

                if (existingUser == null)
                {
                    return NotFound($"User with Id {id} not found.");
                }

                var existingEmailUser = await _userRepository.GetUserByEmail(updateModel.Email);
                if (existingEmailUser != null && existingEmailUser.Id != id)
                {
                    return BadRequest(new { error = "Email address is already registered." });
                }

                var existingUsernameUser = await _userRepository.GetUserByUsername(updateModel.Username);
                if (existingUsernameUser != null && existingUsernameUser.Id != id)
                {
                    return BadRequest(new { error = "Username is already registered." });
                }

                _mapper.Map(updateModel, existingUser);
                await _userRepository.UpdateUser(existingUser);

                var updatedUserDto = _mapper.Map<UserDto>(existingUser);

                return Ok(updatedUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update user with Id {id}.");
                return BadRequest(new { error = $"Failed to update user with Id {id}." });
            }
        }



    }
}
