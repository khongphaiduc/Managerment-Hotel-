using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using Quartz;

namespace Management_Hotel_2025.Modules.Secheduler
{
    public class LateCheckOutCalculator : IJob
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _Inotification;
        private readonly IConfiguration _Iconfi;

        public LateCheckOutCalculator(ManagermentHotelContext dbcontext, INotifications inotification, IConfiguration iconfi)
        {
            _dbcontext = dbcontext;
            _Inotification = inotification;
            _Iconfi = iconfi;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var today = DateTime.Now.Date;

            var lateCheckOuts = await _dbcontext.Bookings
                .Include(s => s.BookingDetails)
                .Where(b => b.Status == "CheckIn")
                .ToListAsync();

            foreach (var item in lateCheckOuts)
            {
                var checkOutDetail = item.BookingDetails.FirstOrDefault();
                if (checkOutDetail?.CheckOutDate == null) continue;

                var checkOutDate = checkOutDetail.CheckOutDate.Value.Date;
                {
                    item.Status = "Overdue";
                }
            }

            await _dbcontext.SaveChangesAsync();
        }
    }
}
