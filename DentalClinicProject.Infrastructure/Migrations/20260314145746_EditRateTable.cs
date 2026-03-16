using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditRateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_AppointmentId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_DoctorId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_ProductId",
                table: "Rates");

            migrationBuilder.DeleteData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Rates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "Rates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Rates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Rates",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "Comment", "CreatedAt", "Discriminator", "ProductId", "Value" },
                values: new object[] { 1, "Good service overall", new DateOnly(2024, 2, 2), "ProductRate", 1, 4 });

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "Comment", "CreatedAt", "Discriminator", "DoctorId", "Value" },
                values: new object[] { 2, "Excellent service and very professional doctor", new DateOnly(2024, 2, 2), "DoctorRate", 1, 3 });

            migrationBuilder.CreateIndex(
                name: "IX_Rates_AppointmentId",
                table: "Rates",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_DoctorId",
                table: "Rates",
                column: "DoctorId",
                unique: true,
                filter: "[DoctorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ProductId",
                table: "Rates",
                column: "ProductId",
                unique: true,
                filter: "[ProductId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_AppointmentId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_DoctorId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_ProductId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Rates");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentId", "Comment", "DoctorId", "Value" },
                values: new object[] { 1, "Excellent service and very professional doctor", 1, 3 });

            migrationBuilder.UpdateData(
                table: "Rates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AppointmentId", "Comment", "CreatedAt", "DoctorId", "ProductId", "Value" },
                values: new object[] { 2, "Good service overall", new DateOnly(2024, 2, 6), 2, 2, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Rates_AppointmentId",
                table: "Rates",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_DoctorId",
                table: "Rates",
                column: "DoctorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ProductId",
                table: "Rates",
                column: "ProductId",
                unique: true);
        }
    }
}
