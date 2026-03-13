using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs.Core.Update
{
    public class UpdateRateDTO
    {
        public int Id { get; set; }
        public RatingCategory? Value { get; set; }
        public string? Comment { get; set; }
    }
}
