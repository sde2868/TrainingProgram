using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TraineeManagement.Models;
using TraineeManagement.Services;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]

    public class UserController(IUser iusr, IConfiguration config) : ControllerBase
    {
        private readonly IUser iuser = iusr;
        private readonly IConfiguration _config = config;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                User? user = await iuser.Login(
                    dto.Username,
                    dto.Password
                );
                if (user == null)
                {
                    return Unauthorized(new
                    {
                        message = "Invalid username or password"
                    });
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

                string jwt = new JwtSecurityTokenHandler()
                    .WriteToken(token);

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
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await iuser.GetAll(search));
        }

        [HttpGet("{id}")]
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
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            User user = await iuser.Create(dto);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDTO dto)
        {
            User? user = await iuser.Put(id, dto);
            return Ok(iuser.ReturnDTO(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            User? user = await iuser.DeleteById(id);
            return NoContent();
        }
    }
}