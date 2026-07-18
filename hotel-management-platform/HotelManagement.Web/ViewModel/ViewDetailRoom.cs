using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MyData.Models;

namespace Management_Hotel_2025.ViewModel
{
    public class ViewDetailRoom
    {
        public int RoomId { get; set; }
        public int RoomTypeId { get; set; }
        public string NameType { get; set; }
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
        public string? PathImage { get; set; }
        public decimal Price { get; set; }
        public string MaxGuests { get; set; }
        public List<string> ListPathImage { get; set; } = new List<string>();
        public IList<Amenity> ListAmenites { get; set; } = new List<Amenity>();
    }
}
