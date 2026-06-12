using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class UserController(IUser iusr, IConfiguration config, ILogger<UserController> logger) : ControllerBase
    {
        private readonly IUser iuser = iusr;
        private readonly IConfiguration _config = config;
        private readonly ILogger<UserController> _logger = logger;

        [HttpPost("/api/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                User? user = await iuser.Login( dto.Username, dto.Password );
                if (user == null)
                {
                    _logger.LogWarning($"User with username {dto.Username} not found!");
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                List<Claim> claims =
                [
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                ];

                SymmetricSecurityKey key = new(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
                );

                SigningCredentials creds = new(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

                DateTime expires = DateTime.UtcNow.AddHours(1);

                JwtSecurityToken token = new(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                string jwt = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation($"User {user.UserName} ({user.Role}) logged in successfully! (Expires in 1 hour)");
                return Ok(new
                {
                    token = jwt,
                    expiresIn = 3600,
                    user = new
                    {
                        id = user.Id,
                        username = user.UserName,
                        role = user.Role.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Login failed for user {dto.Username}");
                throw new Exception("Error during login.", ex);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await iuser.GetAll(search));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            User? user = await iuser.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User Not Found!" });
            }
            return Ok(iuser.ReturnDTO(user));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            User user = await iuser.Create(dto);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDTO dto)
        {
            User? user = await iuser.Put(id, dto);
            return Ok(iuser.ReturnDTO(user));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteById(int id)
        {
            User? user = await iuser.DeleteById(id);
            return NoContent();
        }
    }
}