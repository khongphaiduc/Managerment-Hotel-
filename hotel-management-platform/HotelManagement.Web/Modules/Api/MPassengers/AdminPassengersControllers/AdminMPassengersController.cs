using API_BookingHotel.Modules.MPassengers.AdminPassengersSerives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BookingHotel.Modules.MPassengers.AdminPassengersControllers
{
    [Authorize(Roles ="Admin,Staff")]
    [Route("admin")]
    [ApiController]
    public class AdminMPassengersController : ControllerBase
    {
        private readonly IPassengers _IPassenger;

        public AdminMPassengersController(IPassengers passengers)
        {
            _IPassenger = passengers;
        }



        [HttpGet("passenger/{PassengerCode}")]
// Returns detailed passenger information for the supplied passenger code.
        public async Task<IActionResult> GetPassengerInfo(string PassengerCode)
        {
            var hostapi = $"{Request.Scheme}://{Request.Host.Value}";
            var passenger = await _IPassenger.GetPassengerInfo(PassengerCode, hostapi);

            if (passenger.PassengerCode == "0000")
            {
                return NotFound("Không tìm thấy hành khách với mã đã cho.");
            }
            else
            {
                return Ok(passenger);
            }


        }

    }
}
