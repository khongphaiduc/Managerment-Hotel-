namespace Management_Hotel_2025.Modules.Statistics.StatisticsModels
{
    public class StatisticsViewModel
    {
        public DataYears allRevenuesByYear { get; set; }  // doanh thu của từng năm 

        public DataYears roomRevenuesByYear { get; set; }


        public DataYears serviceRevenuesByYear { get; set; }

        public DataYears allBookingsByYear { get; set; }  // số booking 

        public DataYears genderByYear { get; set; }    // tỉ lệ giới tính 
    }


    public class DataYears
    {
        public Dictionary<int, List<int>> Data { get; set; }

    }
}
