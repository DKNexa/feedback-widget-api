using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using feedbackbackWidget_API.Data;
using feedbackbackWidget_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace feedbackbackWidget_API.Controllers
{
   

        [Route("api/[controller]")]

        [ApiController]

        public class AuthController : ControllerBase

        {

            private readonly FeedbackContext _context;

            private readonly IConfiguration _configuration;

            public AuthController(FeedbackContext context, IConfiguration configuration)

            {

                _context = context;

                _configuration = configuration;

            }

            // POST api/auth/login

            [HttpPost("login")]

            public async Task<IActionResult> Login([FromBody] LoginRequest request)

            {

                // Basic validation

                if (string.IsNullOrEmpty(request.Username))

                    return BadRequest("Username is required");

                if (string.IsNullOrEmpty(request.Password))

                    return BadRequest("Password is required");

                // Find user (in production, compare hashed passwords)

                var user = await _context.Users

                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null || !VerifyPassword(request.Password, user.Password))

                    return Unauthorized("Invalid username or password");

                // Generate JWT token

                var token = GenerateJwtToken(user);

                return Ok(new
                {

                    Username = user.Username,

                    Role = user.Role,

                    Token = token,

                    ExpiresIn = _configuration["Jwt:ExpiryInMinutes"] + " minutes"

                });

            }

            // POST api/auth/register

            [HttpPost("register")]

            public async Task<IActionResult> Register([FromBody] RegisterRequest request)

            {

                // Basic validation

                if (string.IsNullOrEmpty(request.Username))

                    return BadRequest("Username is required");

                if (string.IsNullOrEmpty(request.Password))

                    return BadRequest("Password is required");

                if (string.IsNullOrEmpty(request.Role) || !(request.Role == "Admin" || request.Role == "User"))

                    return BadRequest("Valid Role is required");

                // Check if username exists

                if (await _context.Users.AnyAsync(u => u.Username == request.Username))

                    return BadRequest("Username already exists");

                // Create new user (in production, hash the password)

                var user = new User

                {

                    Username = request.Username,

                    Password = HashPassword(request.Password), // Hash the password

                    Role = request.Role

                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return Ok(new
                {

                    Username = user.Username,

                    Role = user.Role,

                    Message = "Registration successful"

                });

            }

            private string GenerateJwtToken(User user)

            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]

                {

                new Claim(ClaimTypes.NameIdentifier, user.Username),

                new Claim(ClaimTypes.Role, user.Role)

            };

                var token = new JwtSecurityToken(

                    _configuration["Jwt:Issuer"],

                    _configuration["Jwt:Audience"],

                    claims,

                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),

                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

            }

            // In production, use proper password hashing like BCrypt

            private string HashPassword(string password)

            {

                // This is a simple example - use proper hashing in production

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            }

            private bool VerifyPassword(string inputPassword, string storedHash)

            {

                // Compare the hashes

                var inputHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputPassword));

                return inputHash == storedHash;

            }

        }

        // Request models

        public class LoginRequest

        {

            public string Username { get; set; }

            public string Password { get; set; }

        }

        public class RegisterRequest

        {

            public string Username { get; set; }

            public string Password { get; set; }

            public string Role { get; set; } // "Admin" or "User"

        }

    }
