using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasData(new Appointment
            {
                Id = SeedData.Appointment1Id,
                CreatedAt = SeedData.Appointment1CreatedDate,
                DoctorId = SeedData.Doctor1EntityId,
                ExaminationEppointment = SeedData.Appointment1Date,
                PatientId = SeedData.Patient1EntityId,
                ServiceId = SeedData.Service1Id
            });
            builder.HasData(new Appointment
            {
                Id = SeedData.Appointment2Id,
                CreatedAt = SeedData.Appointment2CreatedDate,
                DoctorId = SeedData.Doctor2EntityId,
                ExaminationEppointment = SeedData.Appointment2Date,
                PatientId = SeedData.Patient2EntityId,
                ServiceId = SeedData.Service2Id
            });
        }
    }
}