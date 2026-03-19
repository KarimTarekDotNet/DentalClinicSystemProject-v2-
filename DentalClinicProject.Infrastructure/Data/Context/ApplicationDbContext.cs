using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DentalClinicProject.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<AppointmentRate> AppointmentRates { get; set; }
        public DbSet<ProductRate> ProductRates { get; set; }
        public DbSet<DoctorRate> DoctorRates { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

            builder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.ExaminationEppointment })
                .IsUnique();

            builder.Entity<CartItem>()
                .HasIndex(x => new { x.CartId, x.ProductId })
                .IsUnique();

            builder.Entity<DoctorRate>()
                .HasIndex(r => new { r.UserId, r.DoctorId })
                .IsUnique();

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            builder.Entity<CartItem>()
                .Property(c => c.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Doctor>()
                .Property(d => d.Salary)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

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

            // Service -> Appointments (One-to-Many)
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser -> Orders (One-to-Many)
            builder.Entity<Order>()
                .HasOne(o => o.AppUser)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> OrderItems (One-to-Many)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order -> Payments (One-to-Many)
            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem -> Products (One-to-Many)
            builder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(c => c.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Rate -> Appointment (One-to-One)
            builder.Entity<AppointmentRate>()
                .HasOne(r => r.Appointment)
                .WithMany()
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate -> Product (One-to-One)
            builder.Entity<ProductRate>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate -> Doctor (One-to-One)
            builder.Entity<DoctorRate>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Apply all configurations from assembly (for seed data)
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}