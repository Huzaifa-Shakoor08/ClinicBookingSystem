using ClinicBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
            .Property(d => d.ConsultationFee)
            .HasColumnType("decimal(10,2)");
            // User -> Doctor (one to one)
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId);

            // Doctor -> TimeSlots (one to many)
            modelBuilder.Entity<TimeSlot>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.TimeSlots)
                .HasForeignKey(t => t.DoctorId);

            // Appointment -> Patient (many to one)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Doctor (many to one)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> TimeSlot (one to one)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.TimeSlot)
                .WithOne(t => t.Appointment)
                .HasForeignKey<Appointment>(a => a.SlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}