using API.Helpers;
using API.Model;
using API.Services.ORM;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserService userService;
        public AuthController(IConfiguration config, UserService userService)
        {
            _config = config;
            this.userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userService.GetByUsername(model.Username);
            if (model.Username == null || model.Password == null)
            {
                return BadRequest("Username and password are required.");
            }

            if (user == null || !PasswordHelper.Verify(user.Password, model.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        [HttpPost("login1")]
        public async Task<IActionResult> Login1([FromBody] LoginModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username is required.");
            }
            var user = await userService.GetByUsername(request.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, "JWT Secret Key is not configured.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Tambahkan ID user jika ada
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token()
        {
            string username = _config["AppSettings:APIUsername"] ?? "";
            string password = _config["AppSettings:APIPassword"] ?? "";
            var user = await userService.GetByUsername(username);
            if (user == null || !PasswordHelper.Verify(user.Password, password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }



}
