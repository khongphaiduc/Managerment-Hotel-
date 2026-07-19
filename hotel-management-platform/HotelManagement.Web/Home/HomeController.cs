using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;

namespace Management_Hotel_2025.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ManagermentHotelContext _dbcontext;

        public HomeController(ILogger<HomeController> logger, ManagermentHotelContext context)
        {
            _logger = logger;
            _dbcontext = context;
        }


        [Route("trungducluxuryhotel")]
        // Opens the home page, redirects staff/admin users to their dashboards, and initializes the guest payment session.
        public IActionResult Index()
        {


            if (User.IsInRole("Staff"))
            {
                return RedirectToAction("StaffViewListRoom", "StaffManagementRoom");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminHomePage", "Admin");
            }
            else
            {

                string? payIdentity = HttpContext.Session.GetString("Pay_Identity");

                if (string.IsNullOrEmpty(payIdentity))
                {
                    payIdentity = Guid.NewGuid().ToString("N").Substring(0, 24);
                    HttpContext.Session.SetString("Pay_Identity", payIdentity);
                }
                return View();
            }
        }

        [Route("/")]
    
        public IActionResult Intro()
        {
            return View();
        }

        [Route("trial")]
       
        public IActionResult MemeDevelopment()
        {
            return View();
        }

      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
