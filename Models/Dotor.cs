namespace ClinicBookingSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public decimal ConsultationFee { get; set; }

        public User? User { get; set; }
        public ICollection<TimeSlot>? TimeSlots { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}