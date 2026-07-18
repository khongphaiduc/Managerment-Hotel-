namespace Management_Hotel_2025.Modules.AdminMPassengers.MPassengerModels
{
    public class PassengersInfo
    {
        public string PersonalCode { get; set; } = null!;
        
        public string Nationality { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public int TimesVisited { get; set; }


    }
}
