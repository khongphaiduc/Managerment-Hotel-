namespace API_BookingHotel.Modules.Statistics.StatisticsModels
{
    public class StatisticsViewModel
    {
        public DataYears allRevenuesByYear { get; set; }

        public DataYears roomRevenuesByYear { get; set; }


        public DataYears serviceRevenuesByYear { get; set; }

        public DataYears allBookingsByYear { get; set; }

        public DataYears genderByYear { get; set; }
    }


    public class DataYears
    {
        public Dictionary<int, List<int>> Data { get; set; }

    }

}
