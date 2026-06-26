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
    public class TimeSlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimeSlotsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/timeslots/doctor/1
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorSlots(int doctorId)
        {
            var slots = await _context.TimeSlots
                .Include(t => t.Doctor)
                .ThenInclude(d => d!.User)
                .Where(t => t.DoctorId == doctorId && !t.IsBooked)
                .Select(t => new TimeSlotResponseDTO
                {
                    Id = t.Id,
                    DoctorId = t.DoctorId,
                    DoctorName = t.Doctor!.User!.FullName,
                    SlotDate = t.SlotDate,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    IsBooked = t.IsBooked
                })
                .ToListAsync();

            return Ok(slots);
        }

        // POST: api/timeslots
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateSlot(CreateTimeSlotDTO dto)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == dto.DoctorId);

            if (!doctorExists)
                return NotFound("Doctor not found.");

            var slot = new TimeSlot
            {
                DoctorId = dto.DoctorId,
                SlotDate = dto.SlotDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsBooked = false
            };

            _context.TimeSlots.Add(slot);
            await _context.SaveChangesAsync();

            return Ok("Time slot created successfully.");
        }

        // DELETE: api/timeslots/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var slot = await _context.TimeSlots.FindAsync(id);

            if (slot == null)
                return NotFound("Time slot not found.");

            if (slot.IsBooked)
                return BadRequest("Cannot delete a booked slot.");

            _context.TimeSlots.Remove(slot);
            await _context.SaveChangesAsync();

            return Ok("Time slot deleted successfully.");
        }
    }
}