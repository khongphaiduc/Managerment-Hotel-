using Management_Hotel_2025.Modules.AdminMPassengers.MPassengerModels;

namespace Management_Hotel_2025.Modules.AdminMPassengers.MPassengersServices
{
    public interface IAdminMPassengers
    {
       public Task<List<PassengersInfo>> GetListViewPassengers ();

      

    }
}
