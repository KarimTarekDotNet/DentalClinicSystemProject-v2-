using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 25));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 25));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 25));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 25));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Processing");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 24));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 24));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 24));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateOnly(2026, 3, 24));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Pending");
        }
    }
}
