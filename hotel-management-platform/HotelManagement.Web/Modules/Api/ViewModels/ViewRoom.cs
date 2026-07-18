namespace API_BookingHotel.ViewModels
{
    public class ViewRoom
    {
        public int IdRoom { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }

        public string NumberOfRooms { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }

    }
}
