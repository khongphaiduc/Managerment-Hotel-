namespace API_BookingHotel.Modules.AmentityModules.AmentityServices
{
    public class AmentityUpdate
    {
        public int? AmenityId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? UrlImage { get; set; }
        public string? Status { get; set; }
        public IFormFile? UpdateImage { get; set; }
    }
}
