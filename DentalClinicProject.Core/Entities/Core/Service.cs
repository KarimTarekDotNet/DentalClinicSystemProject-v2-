namespace DentalClinicProject.Core.Entities.Core
{
    public class Service : BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}