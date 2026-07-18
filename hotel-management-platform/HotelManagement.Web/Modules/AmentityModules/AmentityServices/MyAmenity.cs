namespace Management_Hotel_2025.Modules.AmentityModules.AmentityServices
{
    public class MyAmenity
    {
        public int AmenityId { get; set; }
        public string Name { get; set; } = null!;

        public string? Status { get; set; } 

        public string? Description { get; set; }
        public string? UrlImage { get; set; }

        public IFormFile? UpdateImage { get; set; }
    }
}
