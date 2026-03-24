using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Seeding
{
    public static class SeedData
    {
        // ========================
        // Roles
        // ========================
        public const string AdminRole = "Admin";
        public const string DoctorRole = "Doctor";
        public const string PatientRole = "Patient";
        public const string UserRole = "User";
        public const string DeliveryRole = "Delivery";

        // Normalized Roles
        public const string AdminNormalizedRole = "ADMIN";
        public const string DoctorNormalizedRole = "DOCTOR";
        public const string PatientNormalizedRole = "PATIENT";
        public const string UserNormalizedRole = "USER";
        public const string DeliveryNormalizedRole = "DELIVERY";

        // ========================
        // Admin User Data
        // ========================
        public const string AdminUserId = "admin-1";
        public const string AdminUserName = "adminUsername";
        public const string AdminNormalizedUserName = "ADMIN@DENTALCLINIC.COM";
        public const string AdminEmail = "admin@dentalclinic.com";
        public const string AdminNormalizedEmail = "ADMIN@DENTALCLINIC.COM";
        public const string AdminFirstName = "Admin";
        public const string AdminLastName = "Dental";
        public const string AdminPassword = "Admin@123";
        public const string AdminPasswordHash = "AQAAAAIAAYagAAAAEGorgtN1aWOwuhiQlRZuNa7oimeAFA8ZS+yb3u8Qc+C+x3MipZNNNIhOi5bl/1ws+Q==";
        public const string AdminProviderId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
        public const string AdminSecurityStamp = "QURNSU5TRUNVUklUWVNUQU1QMDA="; // Base64 of "ADMINSECURITYSTAMP00"
        public const string AdminConcurrencyStamp = "ADMIN-CONCURRENCY-001";

        // ========================
        // Doctor 1 User Data
        // ========================
        public const string Doctor1UserId = "doctor-1";
        public const string Doctor1UserName = "doctorUsername1";
        public const string Doctor1NormalizedUserName = "DOCTOR1@DENTALCLINIC.COM";
        public const string Doctor1Email = "doctor1@dentalclinic.com";
        public const string Doctor1NormalizedEmail = "DOCTOR1@DENTALCLINIC.COM";
        public const string Doctor1FirstName = "Sarah";
        public const string Doctor1LastName = "Ali";
        public const string Doctor1Password = "Doctor@123";
        public const string Doctor1PasswordHash = "AQAAAAIAAYagAAAAEA6LlozjK/8AFKsEUcoc0URf8Kc3fYa8bLsw6H5+lOvxvwmCGU65CSzZYe3DiUlc1Q==";
        public const string Doctor1ProviderId = "b2c3d4e5-f6a7-8901-bcde-f12345678901";
        public const string Doctor1SecurityStamp = "RE9DVE9SMVNFQ1VSSVRZU1RBTVA="; // Base64 of "DOCTOR1SECURITYSTAMP"
        public const string Doctor1ConcurrencyStamp = "DOCTOR1-CONCURRENCY-001";

        // ========================
        // Doctor 2 User Data
        // ========================
        public const string Doctor2UserId = "doctor-2";
        public const string Doctor2UserName = "doctorUsername2";
        public const string Doctor2NormalizedUserName = "DOCTOR2@DENTALCLINIC.COM";
        public const string Doctor2Email = "doctor2@dentalclinic.com";
        public const string Doctor2NormalizedEmail = "DOCTOR2@DENTALCLINIC.COM";
        public const string Doctor2FirstName = "Mahmoud";
        public const string Doctor2LastName = "Hassan";
        public const string Doctor2Password = "Doctor@123";
        public const string Doctor2PasswordHash = "AQAAAAIAAYagAAAAEIJreR46lGQQQ5gUg3tr2IJZisZshIYLvut+kz5WqIo2zwKjv9NYrZS9xHAhm1k8Bg==";
        public const string Doctor2ProviderId = "c3d4e5f6-a7b8-9012-cdef-123456789012";
        public const string Doctor2SecurityStamp = "RE9DVE9SMlNFQ1VSSVRZU1RBTVA="; // Base64 of "DOCTOR2SECURITYSTAMP"
        public const string Doctor2ConcurrencyStamp = "DOCTOR2-CONCURRENCY-001";

        // ========================
        // Delivery 1 User Data
        // ========================
        public const string Delivery1UserId = "delivery-1";
        public const string Delivery1UserName = "deliveryUsername1";
        public const string Delivery1NormalizedUserName = "DELIVERYUSERNAME1";
        public const string Delivery1Email = "delivery@dentalclinic.com";
        public const string Delivery1NormalizedEmail = "DEKIVERY1@DENTALCLINIC.COM";
        public const string Delivery1FirstName = "Ahmed";
        public const string Delivery1LastName = "Elsayed";
        public const string Delivery1Password = "User@123";
        public const string Delivery1PasswordHash = "AQAAAAIAAYagAAAAEDXDYwydBslF+l+wymoQRV0AhyxRvN5QbAWsAjeGBTb/wHqEv8TlM7PT3h08apHLdQ==";
        public const string Delivery1ProviderId = "b2c3d4e5-f6a7-8901-qwer-f12345678901";
        public const string Delivery1SecurityStamp = "RE9DVE9SMVNFQ1VSSVRZU1RBTVA="; // Base64 of "DOCTOR1SECURITYSTAMP"
        public const string Delivery1ConcurrencyStamp = "DELIVERY1-CONCURRENCY-001";

        // ========================
        // Delivery 2 User Data
        // ========================
        public const string Delivery2UserId = "delivery-2";
        public const string Delivery2UserName = "deliveryUsername2";
        public const string Delivery2NormalizedUserName = "DELIVERYUSERNAME2";
        public const string Delivery2Email = "delivery2@dentalclinic.com";
        public const string Delivery2NormalizedEmail = "DELIVERY2@DENTALCLINIC.COM";
        public const string Delivery2FirstName = "Mahmoud";
        public const string Delivery2LastName = "Hassan";
        public const string Delivery2Password = "User@123";
        public const string Delivery2PasswordHash = "AQAAAAIAAYagAAAAEDXDYwydBslF+l+wymoQRV0AhyxRvN5QbAWsAjeGBTb/wHqEv8TlM7PT3h08apHLdQ==";
        public const string Delivery2ProviderId = "c3d4e5f6-a7b8-9012-asdf-123456789012";
        public const string Delivery2SecurityStamp = "RE9DVE9SMlNFQ1VSSVRZU1RBTVA="; // Base64 of "DOCTOR2SECURITYSTAMP"
        public const string Delivery2ConcurrencyStamp = "DELIVERY2-CONCURRENCY-001";

        // ========================
        // Patient 1 User Data
        // ========================
        public const string Patient1UserId = "patient-1";
        public const string Patient1UserName = "patient1@example.com";
        public const string Patient1NormalizedUserName = "PATIENT1@EXAMPLE.COM";
        public const string Patient1Email = "patient1@example.com";
        public const string Patient1NormalizedEmail = "PATIENT1@EXAMPLE.COM";
        public const string Patient1FirstName = "Fatima";
        public const string Patient1LastName = "Ahmed";
        public const string Patient1Password = "Patient@123";
        public const string Patient1PasswordHash = "AQAAAAIAAYagAAAAEJ/gwoGWiEdWFdk8LsAh1dfUvAu/9GGa3ebgbXpWPWhXjnsXRm0DQ8nLKq/yi55LcA==";
        public const string Patient1ProviderId = "d4e5f6a7-b8c9-0123-def1-234567890123";
        public const string Patient1SecurityStamp = "UEFUSUVOVDFTRUNTVVJJVFVTVEFNUA=="; // Base64 of "PATIENT1SECURITYSTAMP"
        public const string Patient1ConcurrencyStamp = "PATIENT1-CONCURRENCY-001";

        // ========================
        // Patient 2 User Data
        // ========================
        public const string Patient2UserId = "patient-2";
        public const string Patient2UserName = "patient2@example.com";
        public const string Patient2NormalizedUserName = "PATIENT2@EXAMPLE.COM";
        public const string Patient2Email = "patient2@example.com";
        public const string Patient2NormalizedEmail = "PATIENT2@EXAMPLE.COM";
        public const string Patient2FirstName = "Khaled";
        public const string Patient2LastName = "Abdullah";
        public const string Patient2Password = "Patient@123";
        public const string Patient2PasswordHash = "AQAAAAIAAYagAAAAEMk3DV11DFvB+aKgUbP3Q84O++hoXTi1I9ikkt+Via2Rr4VsZHLyvWWw9yUMRQghcg==";
        public const string Patient2ProviderId = "e5f6a7b8-c9d0-1234-ef12-345678901234";
        public const string Patient2SecurityStamp = "UEFUSUVOVDJTRUNTVVJJVFVTVEFNUA=="; // Base64 of "PATIENT2SECURITYSTAMP"
        public const string Patient2ConcurrencyStamp = "PATIENT2-CONCURRENCY-001";

        // ========================
        // Regular User Data
        // ========================
        public const string RegularUserId = "user-1";
        public const string RegularUserName = "user@example.com";
        public const string RegularNormalizedUserName = "USER@EXAMPLE.COM";
        public const string RegularEmail = "user@example.com";
        public const string RegularNormalizedEmail = "USER@EXAMPLE.COM";
        public const string RegularFirstName = "John";
        public const string RegularLastName = "Doe";
        public const string RegularPassword = "User@123";
        public const string RegularPasswordHash = "AQAAAAIAAYagAAAAEDXDYwydBslF+l+wymoQRV0AhyxRvN5QbAWsAjeGBTb/wHqEv8TlM7PT3h08apHLdQ==";
        public const string RegularProviderId = "f6a7b8c9-d0e1-2345-f123-456789012345";
        public const string RegularSecurityStamp = "VVNFUlNFQ1VSSVRZU1RBTVA="; // Base64 of "USERSECURITYSTAMP"
        public const string RegularConcurrencyStamp = "USER-CONCURRENCY-001";

        // ========================
        // Entity IDs
        // ========================
        public const int AdminEntityId = 1;
        public const int Doctor1EntityId = 1;
        public const int Doctor2EntityId = 2;
        public const int Patient1EntityId = 1;
        public const int Patient2EntityId = 2;

        // ========================
        // Dates
        // ========================
        public static readonly DateOnly SeedDate = new(2024, 1, 1);
        public static readonly DateOnly AdminCreatedDate = new(2024, 1, 1);
        public static readonly DateOnly Doctor1CreatedDate = new(2024, 1, 5);
        public static readonly DateOnly Doctor2CreatedDate = new(2024, 1, 10);
        public static readonly DateOnly Patient1CreatedDate = new(2024, 1, 15);
        public static readonly DateOnly Patient2CreatedDate = new(2024, 1, 18);

        // ========================
        // Services Data
        // ========================
        public const int Service1Id = 1;
        public const string Service1Name = "General Checkup";
        public const decimal Service1Price = 200m;
        public const int Service1Duration = 30;

        public const int Service2Id = 2;
        public const string Service2Name = "Teeth Cleaning";
        public const decimal Service2Price = 300m;
        public const int Service2Duration = 45;

        public const int Service3Id = 3;
        public const string Service3Name = "Dental Filling";
        public const decimal Service3Price = 500m;
        public const int Service3Duration = 60;

        public const int Service4Id = 4;
        public const string Service4Name = "Tooth Extraction";
        public const decimal Service4Price = 400m;
        public const int Service4Duration = 30;

        public const int Service5Id = 5;
        public const string Service5Name = "Teeth Whitening";
        public const decimal Service5Price = 1500m;
        public const int Service5Duration = 90;

        public const int Service6Id = 6;
        public const string Service6Name = "Orthodontics";
        public const decimal Service6Price = 8000m;
        public const int Service6Duration = 120;

        // ========================
        // Products Data
        // ========================
        public const int Product1Id = 1;
        public const string Product1Name = "Electric Toothbrush";
        public const string Product1Description = "Advanced electric toothbrush with 3 cleaning modes";
        public const decimal Product1Price = 350m;
        public static readonly DateTime Product1Date = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        public const int Product2Id = 2;
        public const string Product2Name = "Medical Toothpaste";
        public const string Product2Description = "Medical toothpaste for sensitive teeth";
        public const decimal Product2Price = 80m;
        public static readonly DateTime Product2Date = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);


        public const int Product3Id = 3;
        public const string Product3Name = "Dental Floss";
        public const string Product3Description = "Mint flavored dental floss";
        public const decimal Product3Price = 45m;
        public static readonly DateTime Product3Date = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);


        public const int Product4Id = 4;
        public const string Product4Name = "Mouthwash";
        public const string Product4Description = "Antibacterial mouthwash";
        public const decimal Product4Price = 120m;
        public static readonly DateTime Product4Date = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);


        // ========================
        // Appointments Data
        // ========================
        public const int Appointment1Id = 1;
        public static readonly DateTime Appointment1Date = new(2024, 2, 1, 10, 0, 0, DateTimeKind.Utc);
        public static readonly DateOnly Appointment1CreatedDate = new(2024, 1, 25);

        public const int Appointment2Id = 2;
        public static readonly DateTime Appointment2Date = new(2024, 2, 5, 14, 0, 0, DateTimeKind.Utc);
        public static readonly DateOnly Appointment2CreatedDate = new(2024, 1, 26);


        // ========================
        // Orders Data
        // ========================
        public const int Order1Id = 1;
        public const OrderStatus Order1Status = OrderStatus.Completed;

        public const int Order2Id = 2;
        public const OrderStatus Order2Status = OrderStatus.Pending;

        // ========================
        // OrderItems Data
        // ========================
        public const int OrderItem1Id = 1;
        public const string OrderItem1ProductName = "Electric Toothbrush";
        public const decimal OrderItem1Price = 350m;
        public const int OrderItem1Quantity = 1;

        public const int OrderItem2Id = 2;
        public const string OrderItem2ProductName = "Medical Toothpaste";
        public const decimal OrderItem2Price = 80m;
        public const int OrderItem2Quantity = 2;

        public const int OrderItem3Id = 3;
        public const string OrderItem3ProductName = "Dental Floss";
        public const decimal OrderItem3Price = 45m;
        public const int OrderItem3Quantity = 3;

        // ========================
        // Payments Data (Updated)
        // ========================
        public const int Payment1Id = 1;
        public const decimal Payment1Amount = 350m;
        public const string Payment1Currency = "EGP";
        public const string Payment1Description = "Payment for Order #1 - Electric Toothbrush";
        public static readonly DateTime Payment1Date = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        public const PaymentMethod Payment1Method = PaymentMethod.CreditCard;
        public const string Payment1TransactionId = "TXN-2024-001-ORDER-1";

        public const int Payment2Id = 2;
        public const decimal Payment2Amount = 160m;
        public const string Payment2Currency = "EGP";
        public const string Payment2Description = "Payment for Order #2 - Medical Toothpaste x2";
        public static readonly DateTime Payment2Date = new(2024, 1, 20, 14, 15, 0, DateTimeKind.Utc);
        public const PaymentMethod Payment2Method = PaymentMethod.Cash;
        public const string Payment2TransactionId = "TXN-2024-002-ORDER-2";

        // ========================
        // Rates Data
        // ========================
        public const int Rate1Id = 1;
        public const string Rate1Comment = "Excellent service and very professional doctor";
        public static readonly DateOnly Rate1CreatedDate = new(2024, 2, 2);

        public const int Rate2Id = 2;
        public const string Rate2Comment = "Good service overall";
        public static readonly DateOnly Rate2CreatedDate = new(2024, 2, 6);

        // ========================
        // CartItems Data
        // ========================
        public const int CartItem1Id = 1;
        public const decimal CartItem1TotalPrice = 430m;
        public const int CartItem1ItemCount = 2;
        public static readonly DateOnly CartItem1CreatedDate = new(2024, 1, 20);

        public const int CartItem2Id = 2;
        public const decimal CartItem2TotalPrice = 165m;
        public const int CartItem2ItemCount = 2;
        public static readonly DateOnly CartItem2CreatedDate = new(2024, 1, 22);

        // ========================
        // Doctor Settings
        // ========================
        public const decimal Doctor1Salary = 15000m;
        public const int Doctor1Capacity = 10;

        public const decimal Doctor2Salary = 18000m;
        public const int Doctor2Capacity = 8;



    }
}
