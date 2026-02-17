namespace DentalClinicProject.Core.Entities.Users
{
    public class Admin : BaseEntity
    {
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}