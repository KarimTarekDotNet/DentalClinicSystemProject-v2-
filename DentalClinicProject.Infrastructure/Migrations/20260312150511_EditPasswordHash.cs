using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                columns: new[] { "FirstName", "LastName", "PasswordHash" },
                values: new object[] { "Admin", "Dental", "AQAAAAIAAYagAAAAEGorgtN1aWOwuhiQlRZuNa7oimeAFA8ZS+yb3u8Qc+C+x3MipZNNNIhOi5bl/1ws+Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEA6LlozjK/8AFKsEUcoc0URf8Kc3fYa8bLsw6H5+lOvxvwmCGU65CSzZYe3DiUlc1Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIJreR46lGQQQ5gUg3tr2IJZisZshIYLvut+kz5WqIo2zwKjv9NYrZS9xHAhm1k8Bg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ/gwoGWiEdWFdk8LsAh1dfUvAu/9GGa3ebgbXpWPWhXjnsXRm0DQ8nLKq/yi55LcA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMk3DV11DFvB+aKgUbP3Q84O++hoXTi1I9ikkt+Via2Rr4VsZHLyvWWw9yUMRQghcg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDXDYwydBslF+l+wymoQRV0AhyxRvN5QbAWsAjeGBTb/wHqEv8TlM7PT3h08apHLdQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                columns: new[] { "FirstName", "LastName", "PasswordHash" },
                values: new object[] { "Ahmed", "Mohamed", "$2a$12$PIsynfBEoxgQeX.9b1NhK.42bvqcU4z0m6RdOJK1SobWfVPSsx1EO" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-1",
                column: "PasswordHash",
                value: "$2a$12$ItcpdkqaPFmWpAG6LVkGKu8qUspXV7sz4phOKrdAPQtcVN/hPb.tK");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "doctor-2",
                column: "PasswordHash",
                value: "$2a$12$ItcpdkqaPFmWpAG6LVkGKu8qUspXV7sz4phOKrdAPQtcVN/hPb.tK");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-1",
                column: "PasswordHash",
                value: "$2a$12$zFq7IDP.u8zUtqmlrBD55upoDjQFjAn9iPdRNfK95t0rg1fpIwh6S");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "patient-2",
                column: "PasswordHash",
                value: "$2a$12$zFq7IDP.u8zUtqmlrBD55upoDjQFjAn9iPdRNfK95t0rg1fpIwh6S");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1",
                column: "PasswordHash",
                value: "$2a$12$vymAZaoE/iWWVCwN1Cp3DueGCpKrkt3QtrXosVVylxPFSJ31p7a7S");
        }
    }
}
