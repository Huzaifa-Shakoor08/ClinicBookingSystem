using ClinicBookingSystem.Data;
using ClinicBookingSystem.DTOs;
using ClinicBookingSystem.Helpers;
using ClinicBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtHelper = new JwtHelper(config);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            // Validate role
            var allowedRoles = new[] { "Patient", "Doctor", "Admin" };
            if (!allowedRoles.Contains(dto.Role))
                return BadRequest("Invalid role.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = _jwtHelper.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}