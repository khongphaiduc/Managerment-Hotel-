using Management_Hotel_2025.ViewModel;
using Microsoft.Identity.Client;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public interface IOrder
    {

        public Task<Order> ViewOrder(string bookingcode);


        public Task<bool> ConfirmCheckOut(Order order, string OrdersMethod, int idStaff);


        public Task<bool> ConfirmTranfersQRcode(string bookingcode);

    }

}
