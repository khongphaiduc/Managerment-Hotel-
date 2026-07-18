using API_BookingHotel.ViewModels;
using MyData.Models;

namespace API_BookingHotel.Modules.AmentityModules.AmentityServices
{
    public interface IAmenityServices
    {
        public Task<List<AmentityUpdate>> GetAllAmenityAsync(string apihost);

        public Task<AmentityUpdate> GetAmenityByIdAsync(int id, string apihost);


        public Task<bool> CreateAmenityAsync(AmentityUpdate request);


        public Task<bool> UpdateAmenityAsync(AmentityUpdate request);


        public Task<bool> DeleteAmenityAsync(int id);

        public Task<bool> ChangeStatusAmenityAsync(int id);

    }
}
