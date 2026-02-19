using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "role-admin", "ROLE-ADMIN-001", "Admin", "ADMIN" },
                    { "role-doctor", "ROLE-DOCTOR-001", "Doctor", "DOCTOR" },
                    { "role-patient", "ROLE-PATIENT-001", "Patient", "PATIENT" },
                    { "role-user", "ROLE-USER-001", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Provider", "ProviderId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "admin-1", 0, "ADMIN-CONCURRENCY-001", "admin@dentalclinic.com", true, "Ahmed", "Mohamed", false, null, "ADMIN@DENTALCLINIC.COM", "ADMIN@DENTALCLINIC.COM", "$2a$12$PIsynfBEoxgQeX.9b1NhK.42bvqcU4z0m6RdOJK1SobWfVPSsx1EO", null, false, 0, "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "ADMIN-SECURITY-STAMP-001", false, "admin@dentalclinic.com" },
                    { "doctor-1", 0, "DOCTOR1-CONCURRENCY-001", "doctor1@dentalclinic.com", true, "Sarah", "Ali", false, null, "DOCTOR1@DENTALCLINIC.COM", "DOCTOR1@DENTALCLINIC.COM", "$2a$12$ItcpdkqaPFmWpAG6LVkGKu8qUspXV7sz4phOKrdAPQtcVN/hPb.tK", null, false, 0, "b2c3d4e5-f6a7-8901-bcde-f12345678901", "DOCTOR1-SECURITY-STAMP-001", false, "doctor1@dentalclinic.com" },
                    { "doctor-2", 0, "DOCTOR2-CONCURRENCY-001", "doctor2@dentalclinic.com", true, "Mahmoud", "Hassan", false, null, "DOCTOR2@DENTALCLINIC.COM", "DOCTOR2@DENTALCLINIC.COM", "$2a$12$ItcpdkqaPFmWpAG6LVkGKu8qUspXV7sz4phOKrdAPQtcVN/hPb.tK", null, false, 0, "c3d4e5f6-a7b8-9012-cdef-123456789012", "DOCTOR2-SECURITY-STAMP-001", false, "doctor2@dentalclinic.com" },
                    { "patient-1", 0, "PATIENT1-CONCURRENCY-001", "patient1@example.com", true, "Fatima", "Ahmed", false, null, "PATIENT1@EXAMPLE.COM", "PATIENT1@EXAMPLE.COM", "$2a$12$zFq7IDP.u8zUtqmlrBD55upoDjQFjAn9iPdRNfK95t0rg1fpIwh6S", null, false, 0, "d4e5f6a7-b8c9-0123-def1-234567890123", "PATIENT1-SECURITY-STAMP-001", false, "patient1@example.com" },
                    { "patient-2", 0, "PATIENT2-CONCURRENCY-001", "patient2@example.com", true, "Khaled", "Abdullah", false, null, "PATIENT2@EXAMPLE.COM", "PATIENT2@EXAMPLE.COM", "$2a$12$zFq7IDP.u8zUtqmlrBD55upoDjQFjAn9iPdRNfK95t0rg1fpIwh6S", null, false, 0, "e5f6a7b8-c9d0-1234-ef12-345678901234", "PATIENT2-SECURITY-STAMP-001", false, "patient2@example.com" },
                    { "user-1", 0, "USER-CONCURRENCY-001", "user@example.com", true, "John", "Doe", false, null, "USER@EXAMPLE.COM", "USER@EXAMPLE.COM", "$2a$12$vymAZaoE/iWWVCwN1Cp3DueGCpKrkt3QtrXosVVylxPFSJ31p7a7S", null, false, 0, "f6a7b8c9-d0e1-2345-f123-456789012345", "USER-SECURITY-STAMP-001", false, "user@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, new DateOnly(2024, 1, 25), "Advanced electric toothbrush with 3 cleaning modes", "Electric Toothbrush", 350m },
                    { 2, new DateOnly(2024, 1, 26), "Medical toothpaste for sensitive teeth", "Medical Toothpaste", 80m },
                    { 3, new DateOnly(2024, 1, 26), "Mint flavored dental floss", "Dental Floss", 45m },
                    { 4, new DateOnly(2024, 1, 26), "Antibacterial mouthwash", "Mouthwash", 120m }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "DurationInMinutes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, new DateOnly(2024, 2, 2), 30, "General Checkup", 200m },
                    { 2, new DateOnly(2024, 2, 6), 45, "Teeth Cleaning", 300m },
                    { 3, new DateOnly(2024, 2, 6), 60, "Dental Filling", 500m },
                    { 4, new DateOnly(2024, 2, 6), 30, "Tooth Extraction", 400m },
                    { 5, new DateOnly(2024, 2, 6), 90, "Teeth Whitening", 1500m }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "AppUserId", "CreatedAt" },
                values: new object[] { 1, "admin-1", new DateOnly(2024, 1, 1) });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "AppUserId", "CapactiyOfDay", "CreatedAt", "IsApproved", "ReasonForRejection", "Salary" },
                values: new object[,]
                {
                    { 1, "doctor-1", 10, new DateOnly(2024, 1, 5), true, null, 15000m },
                    { 2, "doctor-2", 8, new DateOnly(2024, 1, 10), false, "He did not submit the required documents.", 0.0m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "Currency", "CustomerId", "Description", "PaymentDate", "PaymentMethod", "ProductId", "Status", "TransactionId" },
                values: new object[,]
                {
                    { 1, 350m, "EGP", "user-1", "Purchase of Electric Toothbrush", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Credit Card", 1, 2, "TXN-2024-001-ELECTRIC-BRUSH" },
                    { 2, 80m, "EGP", "user-1", "Purchase of Medical Toothpaste", new DateTime(2024, 1, 20, 14, 15, 0, 0, DateTimeKind.Utc), "Cash", 2, 5, "TXN-2024-002-TOOTHPASTE" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "AppUserId", "CreatedAt", "DoctorId" },
                values: new object[,]
                {
                    { 1, "patient-1", new DateOnly(2024, 1, 15), 1 },
                    { 2, "patient-2", new DateOnly(2024, 1, 18), 1 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "CreatedAt", "DoctorId", "ExaminationEppointment", "PatientId", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2024, 1, 25), 1, new DateTime(2024, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateOnly(2024, 1, 26), 2, new DateTime(2024, 2, 5, 14, 0, 0, 0, DateTimeKind.Utc), 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "DoctorId", "ProductId", "Value" },
                values: new object[,]
                {
                    { 1, 1, "Excellent service and very professional doctor", new DateOnly(2024, 2, 2), 1, 1, 3 },
                    { 2, 2, "Good service overall", new DateOnly(2024, 2, 6), 2, 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-doctor");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-patient");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-user");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1");

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2");

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1");
        }
    }
}
