using Management_Hotel_2025.Modules.Payment.PayOSPayments;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Games.GameControllers
{
    [Route("games")]
    public class GamesHenXuiController : Controller
    {
        private readonly IGameBlackRed _blackred;

        public GamesHenXuiController(IGameBlackRed gameBlackRed)
        {
            _blackred = gameBlackRed;
        }


        public IActionResult PlayGame()
        {
            return View();
        }




        //  hiển thị giao diện nạp xu 
        [HttpGet("napxu")]
        public IActionResult napxu()
        {
            return View();
        }



        //lấy số lượng xu của thằng user 
        [HttpGet("totalcoinuser/{idUser}")]
        public async Task<IActionResult> TotalCoinUser(int idUser)
        {

            decimal totalcoin = await _blackred.GetCoinsForUsers(idUser);


            return Ok(new { coin = totalcoin });
        }

    }
}
