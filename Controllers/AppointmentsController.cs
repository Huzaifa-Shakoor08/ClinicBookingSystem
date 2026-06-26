using ClinicBookingSystem.Data;
using ClinicBookingSystem.DTOs;
using ClinicBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appointments = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d!.User)
                .Include(a => a.TimeSlot)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == userId)
                .Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    PatientName = a.Patient!.FullName,
                    DoctorName = a.Doctor!.User!.FullName,
                    Specialty = a.Doctor.Specialty,
                    SlotDate = a.TimeSlot!.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,
                    Status = a.Status,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: api/appointments/doctor
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return NotFound("Doctor profile not found.");

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .Include(a => a.Doctor).ThenInclude(d => d!.User)
                .Where(a => a.DoctorId == doctor.Id)
                .Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    PatientName = a.Patient!.FullName,
                    DoctorName = a.Doctor!.User!.FullName,
                    Specialty = a.Doctor.Specialty,
                    SlotDate = a.TimeSlot!.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,
                    Status = a.Status,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // POST: api/appointments
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment(CreateAppointmentDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var slot = await _context.TimeSlots.FindAsync(dto.SlotId);
            if (slot == null || slot.IsBooked)
                return BadRequest("Slot not available.");

            var appointment = new Appointment
            {
                PatientId = userId,
                DoctorId = dto.DoctorId,
                SlotId = dto.SlotId,
                Notes = dto.Notes,
                Status = "Pending"
            };

            slot.IsBooked = true;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok("Appointment booked successfully.");
        }

        // PUT: api/appointments/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound("Appointment not found.");

            var allowedStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
            if (!allowedStatuses.Contains(status))
                return BadRequest("Invalid status.");

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return Ok("Appointment status updated.");
        }

        // DELETE: api/appointments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appointment = await _context.Appointments
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == userId);

            if (appointment == null)
                return NotFound("Appointment not found.");

            if (appointment.Status == "Completed")
                return BadRequest("Cannot cancel a completed appointment.");

            appointment.Status = "Cancelled";
            appointment.TimeSlot!.IsBooked = false;

            await _context.SaveChangesAsync();

            return Ok("Appointment cancelled successfully.");
        }
    }
}