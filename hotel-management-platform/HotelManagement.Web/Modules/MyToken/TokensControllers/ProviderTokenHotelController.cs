using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.MyToken.TokensControllers
{
    public class ProviderTokenHotelController : Controller
    {

        [Authorize(Roles ="Admin")]
// Displays the page used to request or inspect hotel token information.
        public IActionResult ViewGetInfo()
        {
            return View();
        }
    }
}
