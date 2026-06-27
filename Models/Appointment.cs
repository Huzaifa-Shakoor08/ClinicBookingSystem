using ClinicBookingSystem.Models;
namespace ClinicBookingSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int SlotId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public TimeSlot? TimeSlot { get; set; }
    }
}