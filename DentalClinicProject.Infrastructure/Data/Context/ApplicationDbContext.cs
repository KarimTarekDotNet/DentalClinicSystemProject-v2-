using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Service> Services { get; set; }

        // User Db Set
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseCollation("Arabic_CI_AI");

            // Configure AppUser
            builder.Entity<AppUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL"); // Only enforce uniqueness for non-null values

            // Configure Relationships

            // AppUser -> Admin (One-to-One)
            builder.Entity<Admin>()
                .HasOne(a => a.AppUser)
                .WithOne()
                .HasForeignKey<Admin>(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser -> Refresh Token (One-to-One)
            builder.Entity<RefreshToken>()
                .HasOne(a => a.AppUser)
                .WithOne()
                .HasForeignKey<RefreshToken>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser -> Doctor (One-to-One)
            builder.Entity<Doctor>()
                .HasOne(d => d.AppUser)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser -> Patient (One-to-One)
            builder.Entity<Patient>()
                .HasOne(p => p.AppUser)
                .WithOne()
                .HasForeignKey<Patient>(p => p.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor -> Patients (One-to-Many) - Optional
            builder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Doctor -> Appointments (One-to-Many)
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient -> Appointments (One-to-Many)
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service -> Appointments (One-to-One)
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithOne()
                .HasForeignKey<Appointment>(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product -> Payment (One-to-One)
            builder.Entity<Product>()
                .HasOne(p => p.Payment)
                .WithOne(pay => pay.Product)
                .HasForeignKey<Payment>(pay => pay.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rate -> Appointment (One-to-One)
            builder.Entity<Rate>()
                .HasOne(r => r.Appointment)
                .WithOne()
                .HasForeignKey<Rate>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate -> Product (One-to-One)
            builder.Entity<Rate>()
                .HasOne(r => r.Product)
                .WithOne()
                .HasForeignKey<Rate>(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate -> Doctor (One-to-One)
            builder.Entity<Rate>()
                .HasOne(r => r.Doctor)
                .WithOne()
                .HasForeignKey<Rate>(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Apply all configurations from assembly (for seed data)
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}