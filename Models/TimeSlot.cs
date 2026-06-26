using ClinicBookingSystem.Models;
namespace ClinicBookingSystem.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateOnly SlotDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; } = false;

        public Doctor? Doctor { get; set; }
        public Appointment? Appointment { get; set; }
    }
}