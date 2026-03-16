using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DentalClinicProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CapactiyOfDay = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRejection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctors_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    ExaminationEppointment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rates_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rates_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    { "admin-1", 0, "ADMIN-CONCURRENCY-001", "admin@dentalclinic.com", true, "Admin", "Dental", false, null, "ADMIN@DENTALCLINIC.COM", "ADMIN@DENTALCLINIC.COM", "AQAAAAIAAYagAAAAEGorgtN1aWOwuhiQlRZuNa7oimeAFA8ZS+yb3u8Qc+C+x3MipZNNNIhOi5bl/1ws+Q==", null, false, "Local", "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "QURNSU5TRUNVUklUWVNUQU1QMDA=", false, "admin@dentalclinic.com" },
                    { "doctor-1", 0, "DOCTOR1-CONCURRENCY-001", "doctor1@dentalclinic.com", true, "Sarah", "Ali", false, null, "DOCTOR1@DENTALCLINIC.COM", "DOCTOR1@DENTALCLINIC.COM", "AQAAAAIAAYagAAAAEA6LlozjK/8AFKsEUcoc0URf8Kc3fYa8bLsw6H5+lOvxvwmCGU65CSzZYe3DiUlc1Q==", null, false, "Local", "b2c3d4e5-f6a7-8901-bcde-f12345678901", "RE9DVE9SMVNFQ1VSSVRZU1RBTVA=", false, "doctor1@dentalclinic.com" },
                    { "doctor-2", 0, "DOCTOR2-CONCURRENCY-001", "doctor2@dentalclinic.com", true, "Mahmoud", "Hassan", false, null, "DOCTOR2@DENTALCLINIC.COM", "DOCTOR2@DENTALCLINIC.COM", "AQAAAAIAAYagAAAAEIJreR46lGQQQ5gUg3tr2IJZisZshIYLvut+kz5WqIo2zwKjv9NYrZS9xHAhm1k8Bg==", null, false, "Local", "c3d4e5f6-a7b8-9012-cdef-123456789012", "RE9DVE9SMlNFQ1VSSVRZU1RBTVA=", false, "doctor2@dentalclinic.com" },
                    { "patient-1", 0, "PATIENT1-CONCURRENCY-001", "patient1@example.com", true, "Fatima", "Ahmed", false, null, "PATIENT1@EXAMPLE.COM", "PATIENT1@EXAMPLE.COM", "AQAAAAIAAYagAAAAEJ/gwoGWiEdWFdk8LsAh1dfUvAu/9GGa3ebgbXpWPWhXjnsXRm0DQ8nLKq/yi55LcA==", null, false, "Local", "d4e5f6a7-b8c9-0123-def1-234567890123", "UEFUSUVOVDFTRUNTVVJJVFVTVEFNUA==", false, "patient1@example.com" },
                    { "patient-2", 0, "PATIENT2-CONCURRENCY-001", "patient2@example.com", true, "Khaled", "Abdullah", false, null, "PATIENT2@EXAMPLE.COM", "PATIENT2@EXAMPLE.COM", "AQAAAAIAAYagAAAAEMk3DV11DFvB+aKgUbP3Q84O++hoXTi1I9ikkt+Via2Rr4VsZHLyvWWw9yUMRQghcg==", null, false, "Local", "e5f6a7b8-c9d0-1234-ef12-345678901234", "UEFUSUVOVDJTRUNTVVJJVFVTVEFNUA==", false, "patient2@example.com" },
                    { "user-1", 0, "USER-CONCURRENCY-001", "user@example.com", true, "John", "Doe", false, null, "USER@EXAMPLE.COM", "USER@EXAMPLE.COM", "AQAAAAIAAYagAAAAEDXDYwydBslF+l+wymoQRV0AhyxRvN5QbAWsAjeGBTb/wHqEv8TlM7PT3h08apHLdQ==", null, false, "Local", "f6a7b8c9-d0e1-2345-f123-456789012345", "VVNFUlNFQ1VSSVRZU1RBTVA=", false, "user@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, new DateOnly(2024, 1, 25), "Advanced electric toothbrush with 3 cleaning modes", "Electric Toothbrush", 350m },
                    { 2, new DateOnly(2024, 1, 25), "Medical toothpaste for sensitive teeth", "Medical Toothpaste", 80m },
                    { 3, new DateOnly(2024, 1, 25), "Mint flavored dental floss", "Dental Floss", 45m },
                    { 4, new DateOnly(2024, 1, 25), "Antibacterial mouthwash", "Mouthwash", 120m }
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
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "role-admin", "admin-1" });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2024, 1, 20), "patient-1" },
                    { 2, new DateOnly(2024, 1, 22), "user-1" }
                });

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
                    { 1, 350m, "EGP", "user-1", "Purchase of Electric Toothbrush", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Credit Card", 1, "Paid", "TXN-2024-001-ELECTRIC-BRUSH" },
                    { 2, 80m, "EGP", "user-1", "Purchase of Medical Toothpaste", new DateTime(2024, 1, 20, 14, 15, 0, 0, DateTimeKind.Utc), "Cash", 2, "Refunded", "TXN-2024-002-TOOTHPASTE" }
                });

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "Comment", "CreatedAt", "Discriminator", "ProductId", "UserId", "Value" },
                values: new object[] { 1, "Good service overall", new DateOnly(2024, 2, 2), "ProductRate", 1, "patient-1", 4 });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CartId", "CreatedAt", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, new DateOnly(2026, 3, 16), 1, 1, 350m },
                    { 2, 1, new DateOnly(2026, 3, 16), 2, 1, 80m },
                    { 3, 2, new DateOnly(2026, 3, 16), 3, 1, 45m },
                    { 4, 2, new DateOnly(2026, 3, 16), 4, 1, 120m }
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
                table: "Rates",
                columns: new[] { "Id", "Comment", "CreatedAt", "Discriminator", "DoctorId", "UserId", "Value" },
                values: new object[] { 2, "Excellent service and very professional doctor", new DateOnly(2024, 2, 2), "DoctorRate", 1, "patient-1", 3 });

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
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "Discriminator", "UserId", "Value" },
                values: new object[] { 3, 1, "Good service overall", new DateOnly(2024, 2, 2), "AppointmentRate", "patient-1", 4 });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AppUserId",
                table: "Admins",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId_ExaminationEppointment",
                table: "Appointments",
                columns: new[] { "DoctorId", "ExaminationEppointment" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AppUserId",
                table: "Doctors",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AppUserId",
                table: "Patients",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProductId",
                table: "Payments",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_AppointmentId",
                table: "Rates",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_DoctorId",
                table: "Rates",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ProductId",
                table: "Rates",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_UserId_DoctorId",
                table: "Rates",
                columns: new[] { "UserId", "DoctorId" },
                unique: true,
                filter: "[DoctorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
