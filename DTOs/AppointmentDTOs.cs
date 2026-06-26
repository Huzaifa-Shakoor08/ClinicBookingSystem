namespace ClinicBookingSystem.DTOs
{
    public class CreateAppointmentDTO
    {
        public int DoctorId { get; set; }
        public int SlotId { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentResponseDTO
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public DateOnly SlotDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}