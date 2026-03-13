using DentalClinicProject.Core.Entities.Core;
using System.Text.Json.Serialization;

namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record CartItemDTO
    {
        public List<int> ProductIds { get; set; } = null!;
        public List<string> Products { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public int ItemCount { get; set; }
        [JsonIgnore]
        public string UserId { get; set; } = null!;
    }
}