using ClinicBookingSystem.Data;
using ClinicBookingSystem.DTOs;
using ClinicBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new DoctorResponseDTO
                {
                    Id = d.Id,
                    FullName = d.User!.FullName,
                    Email = d.User.Email,
                    Specialty = d.Specialty,
                    Bio = d.Bio,
                    ProfileImage = d.ProfileImage,
                    ConsultationFee = d.ConsultationFee
                })
                .ToListAsync();

            return Ok(doctors);
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.Id == id)
                .Select(d => new DoctorResponseDTO
                {
                    Id = d.Id,
                    FullName = d.User!.FullName,
                    Email = d.User.Email,
                    Specialty = d.Specialty,
                    Bio = d.Bio,
                    ProfileImage = d.ProfileImage,
                    ConsultationFee = d.ConsultationFee
                })
                .FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound("Doctor not found.");

            return Ok(doctor);
        }

        // POST: api/doctors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor(CreateDoctorDTO dto)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.UserId && u.Role == "Doctor");

            if (!userExists)
                return BadRequest("User not found or is not registered as a Doctor.");

            var doctor = new Doctor
            {
                UserId = dto.UserId,
                Specialty = dto.Specialty,
                Bio = dto.Bio,
                ProfileImage = dto.ProfileImage,
                ConsultationFee = dto.ConsultationFee
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok("Doctor created successfully.");
        }

        // PUT: api/doctors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, CreateDoctorDTO dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound("Doctor not found.");

            doctor.Specialty = dto.Specialty;
            doctor.Bio = dto.Bio;
            doctor.ProfileImage = dto.ProfileImage;
            doctor.ConsultationFee = dto.ConsultationFee;

            await _context.SaveChangesAsync();

            return Ok("Doctor updated successfully.");
        }

        // DELETE: api/doctors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound("Doctor not found.");

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return Ok("Doctor deleted successfully.");
        }
    }
}