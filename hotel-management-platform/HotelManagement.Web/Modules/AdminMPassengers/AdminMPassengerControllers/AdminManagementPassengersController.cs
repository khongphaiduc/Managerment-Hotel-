
using Management_Hotel_2025.Modules.AdminMPassengers.MPassengersServices;
using ApiPassengers = API_BookingHotel.Modules.MPassengers.AdminPassengersSerives;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Mydata.Models;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers
{

    [Route("admin")]
    public class AdminManagementPassengersController : Controller
    {
        private readonly IAdminMPassengers _IadminMPassgers;
        private readonly ApiPassengers.IPassengers _passengerService;

        public AdminManagementPassengersController(IAdminMPassengers admin, ApiPassengers.IPassengers passengerService)
        {
            _IadminMPassgers = admin;
            _passengerService = passengerService;
        }


        [HttpGet("passengers")]
        // Loads the passenger list for the administrator's passenger-management page.
        public async Task<IActionResult> ViewListPassenger()
        {

            var listPassengers = await _IadminMPassgers.GetListViewPassengers();

            return View(listPassengers);
        }

     
        [HttpGet("passengers/{codePassenger}")]
        // Loads a passenger by code and displays the passenger's personal and contact information.
        public async Task<IActionResult> GetPassengersInfo(string codePassenger)
        {

            var item = await _passengerService.GetPassengerInfo(codePassenger, $"{Request.Scheme}://{Request.Host}");
            if (item.PassengerCode == "0000") return NotFound("Không tìm thấy hành khách");

            return View(new PassengerDetail
            {
                PassengerCode = item.PassengerCode,
                FullName = item.FullName,
                Phone = item.Phone,
                Email = item.Email,
                Address = item.Address,
                Bithday = item.Bithday,
                Sex = item.Sex,
                Nationality = item.Nationality,
                UrlImage = item.UrlImage
            });
        }

    }
}
