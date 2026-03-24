using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockProductInSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Stock",
                value: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Stock",
                value: 0);
        }
    }
}
