using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.MyToken.TokensControllers
{
    public class ProviderTokenHotelController : Controller
    {

        [Authorize(Roles ="Admin")]
        // Handles the ViewGetInfo action.
        public IActionResult ViewGetInfo()
        {
            return View();
        }
    }
}
