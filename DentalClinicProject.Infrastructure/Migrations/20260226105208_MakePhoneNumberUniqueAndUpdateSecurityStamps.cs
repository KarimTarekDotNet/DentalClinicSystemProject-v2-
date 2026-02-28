using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePhoneNumberUniqueAndUpdateSecurityStamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up duplicate phone numbers - keep only the one with PhoneNumberConfirmed = 1
            migrationBuilder.Sql(@"
                -- Delete duplicate phone numbers, keeping only the verified one
                WITH CTE AS (
                    SELECT Id, PhoneNumber, PhoneNumberConfirmed,
                           ROW_NUMBER() OVER (PARTITION BY PhoneNumber ORDER BY PhoneNumberConfirmed DESC, Id) as rn
                    FROM AspNetUsers
                    WHERE PhoneNumber IS NOT NULL
                )
                DELETE FROM AspNetUsers
                WHERE Id IN (SELECT Id FROM CTE WHERE rn > 1)
            ");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                column: "SecurityStamp",
                value: "QURNSU5TRUNVUklUWVNUQU1QMDA=");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1",
                column: "SecurityStamp",
                value: "RE9DVE9SMVNFQ1VSSVRZU1RBTVA=");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2",
                column: "SecurityStamp",
                value: "RE9DVE9SMlNFQ1VSSVRZU1RBTVA=");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1",
                column: "SecurityStamp",
                value: "UEFUSUVOVDFTRUNTVVJJVFVTVEFNUA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2",
                column: "SecurityStamp",
                value: "UEFUSUVOVDJTRUNTVVJJVFVTVEFNUA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1",
                column: "SecurityStamp",
                value: "VVNFUlNFQ1VSSVRZU1RBTVA=");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
    }
}
