namespace ClinicBookingSystem.DTOs
{
    public class CreateDoctorDTO
    {
        public int UserId { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public decimal ConsultationFee { get; set; }
    }

    public class DoctorResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public decimal ConsultationFee { get; set; }
    }
}