namespace API_BookingHotel.Modules.MPassengers.AdminPassengersModels
{
    public class PassengerInfo
    {
        public string PassengerCode { get; set; } = null!;
        public string? FullName { get; set; }//
        public string? Name => FullName?.Split(' ').LastOrDefault();
        public string? Phone { get; set; }//
        public string? Email { get; set; }//
        public string? Address { get; set; } //
        public DateTime? Bithday { get; set; }

        public string? Sex { get; set; }
        public string? Nationality { get; set; }

        public string? UrlImage { get; set; }

    }
}
