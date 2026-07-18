
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyData.Models
{
    [Table("Amenities")]
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? UrlImage { get; set; }
        public string? Description { get; set; }

        public string? status { get; set; }

        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    }
}
