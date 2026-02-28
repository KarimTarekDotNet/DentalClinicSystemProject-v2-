using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSecurityStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                column: "SecurityStamp",
                value: "ADMINSECURITYSTAMP001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1",
                column: "SecurityStamp",
                value: "DOCTOR1SECURITYSTAMP001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2",
                column: "SecurityStamp",
                value: "DOCTOR2SECURITYSTAMP001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1",
                column: "SecurityStamp",
                value: "PATIENT1SECURITYSTAMP001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2",
                column: "SecurityStamp",
                value: "PATIENT2SECURITYSTAMP001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1",
                column: "SecurityStamp",
                value: "USERSECURITYSTAMP001");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                column: "SecurityStamp",
                value: "ADMIN-SECURITY-STAMP-001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1",
                column: "SecurityStamp",
                value: "DOCTOR1-SECURITY-STAMP-001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2",
                column: "SecurityStamp",
                value: "DOCTOR2-SECURITY-STAMP-001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1",
                column: "SecurityStamp",
                value: "PATIENT1-SECURITY-STAMP-001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2",
                column: "SecurityStamp",
                value: "PATIENT2-SECURITY-STAMP-001");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1",
                column: "SecurityStamp",
                value: "USER-SECURITY-STAMP-001");
        }
    }
}
