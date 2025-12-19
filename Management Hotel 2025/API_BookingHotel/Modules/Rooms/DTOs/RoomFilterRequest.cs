namespace API_BookingHotel.Modules.Rooms.DTOs
{
    //int PageCurrent, int NumerItemOfPage, int? Floor, int? PriceMin, int? PriceMax, int? Person, string? StartDate, string? EndDate
    public class RoomFilterRequest
    {

        public int PageCurrent { get; set; }

        public int NumerItemOfPage { get; set; } = 8;

        public int? Floor { get; set; }

        public int? PriceMin { get; set; }

        public int? PriceMax { get; set; }


        public int? Person { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

    }
}
