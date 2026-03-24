using DentalClinicProject.Core.Enum;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicProject.Core.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { Role.Admin.ToString(), Role.Doctor.ToString(), Role.DelivaryMan.ToString(),
                               Role.User.ToString(), Role.Patient.ToString()};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
