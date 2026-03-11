using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs.Core.Update
{
    public class UpdateRateDTO
    {
        public RatingCategory? Value { get; set; }
        public string? Comment { get; set; }
    }
}
