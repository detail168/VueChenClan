using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Login endpoint for JWT authentication
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return BadRequest(new { message = "Email and password are required" });

                // For development, accept any non-empty credentials
                // Extract username from email (part before @)
                string username = request.Email.Contains("@") ? request.Email.Split('@')[0] : request.Email;
                string userId = "dev-user-" + System.Math.Abs(request.Email.GetHashCode());
                
                var testUser = new ApplicationUser 
                { 
                    Id = userId,
                    Email = request.Email, 
                    UserName = username
                };
                
                string token = GenerateJwtToken(testUser);
                
                return Ok(new
                {
                    token = token,
                    refreshToken = "",
                    user = new
                    {
                        id = userId,
                        email = request.Email,
                        name = username
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = "Login error", error = ex.Message });
            }
        }

        /// <summary>
        /// Generate JWT token for user
        /// </summary>
        private string GenerateJwtToken(ApplicationUser user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                // Use a default secret key if not configured (at least 32 characters for HMAC256)
                string secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-at-least-32-characters-long-here";
                
                // Ensure the secret key is long enough (32 characters minimum for HMAC256)
                if (secretKey.Length < 32)
                {
                    secretKey = secretKey.PadRight(32, '*');
                }
                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Name, user.UserName ?? "")
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"] ?? "ChenClanApp",
                    audience: jwtSettings["Audience"] ?? "ChenClanUsers",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token generation error: {ex.Message}");
                throw;
            }
        }
    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
