namespace DentalClinicProject.Core.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}