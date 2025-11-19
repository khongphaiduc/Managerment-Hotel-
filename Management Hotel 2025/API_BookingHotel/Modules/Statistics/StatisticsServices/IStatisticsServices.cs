using API_BookingHotel.Modules.Statistics.StatisticsModels;

namespace API_BookingHotel.Modules.Statistics.StatisticsServices
{
    public interface IStatisticsServices
    {

        Task<StatisticsViewModel> GetStatisticsAsync();    //lấy thông kế của khách sạn

    }
}
