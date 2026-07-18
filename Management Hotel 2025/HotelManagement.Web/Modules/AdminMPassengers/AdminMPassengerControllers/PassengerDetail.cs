namespace Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers
{
    public class PassengerDetail
    {
        public string PassengerCode { get; set; } = null!;
        public string? FullName { get; set; }//
        public string? Name { get; }//
        public string? Phone { get; set; }//
        public string? Email { get; set; }//
        public string? Address { get; set; } //
        public DateTime? Bithday { get; set; }

        public string? Sex { get; set; }
        public string? Nationality { get; set; }

        public string? UrlImage { get; set; }
    }
}
