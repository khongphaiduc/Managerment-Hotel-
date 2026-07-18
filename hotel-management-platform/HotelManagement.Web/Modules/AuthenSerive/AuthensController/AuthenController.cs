

using Management_Hotel_2025.Serives.AuthenSerive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Mydata.Models;
using System.Threading.Tasks;
using Management_Hotel_2025.Serives.GenarateToken;

namespace Management_Hotel_2025.Modules.AuthenSerive.AuthensController
{
    public class AuthenController : Controller
    {
        private readonly ManagermentHotelContext _dbContext;
        private readonly RegisterAccount _MyRegister;
        private readonly ValidationAuthen _Validation;
        private readonly Login _Login;
        private readonly ILogger<AuthenController> _Logger;
        private readonly GenarateTokenHotel _tokenGenerator;

        public AuthenController(ManagermentHotelContext dbcontext, RegisterAccount MyRegister, ValidationAuthen Validation, Login login, ILogger<AuthenController> logger, GenarateTokenHotel tokenGenerator)
        {
            _dbContext = dbcontext;
            _MyRegister = MyRegister;
            _Validation = Validation;
            _Login = login;
            _Logger = logger;
            _tokenGenerator = tokenGenerator;
        }



        private Task SaveTokenSession(int userId)
        {
            var token = _tokenGenerator.GetGenarateTokenHotel(userId, 60);
            HttpContext.Session.SetString("token", token);
            return Task.CompletedTask;
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] User users)
        {
            string email = users.Email;
            string password = users.PasswordHash;

            var result = _Login.MyLogin(email, password);
            if (!result)
            {
                return Json(new { success = false });
            }

            var IdUser = (from a in _dbContext.Users
                          where a.Email == users.Email
                          select a.UserId).FirstOrDefault();


            var userFromDb = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (userFromDb == null)
            {
                return Json(new { success = false });
            }

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, userFromDb.Email),
               new Claim(ClaimTypes.Role, userFromDb.Role),
               new Claim("FullName", userFromDb.FullName),
               new Claim("IdUser", IdUser.ToString()),
               new Claim(ClaimTypes.NameIdentifier, IdUser.ToString())
            };






            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await SaveTokenSession(IdUser);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);




            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RegisterAccount(User Users)
        {


            string NewAccount = Users.Email;
            string NewPassword = Request.Form["Password"];
            string Phone = Users.PhoneNumber;
            string ConfirmPassword = Request.Form["ConfirmPassword"];



            if (NewPassword != null && !NewPassword.Equals(ConfirmPassword))
            {
                ViewBag.Error = "Password and Confirm Password do not match.";
                return View(Users);
            }
            else if (Users.PhoneNumber != null && _Validation.ExistPhoneNumber(Users.PhoneNumber))
            {
                ViewBag.Error = "Sá»‘ Ä‘iá»‡n thoáº¡i Ä‘Ã£ Ä‘Æ°á»£c sá»­ dá»¥ng, vui lÃ²ng nháº­p sá»‘ khÃ¡c.";
                return View(Users);
            }
            else if (!_Validation.ValidateEmail(NewAccount))
            {
                ViewBag.Error = "Email khÃ´ng há»£p lá»‡.";
                return View(Users);
            }
            else if (NewPassword != null && !_Validation.ValidatePassword(NewPassword))
            {
                ViewBag.Error = "Máº­t kháº©u pháº£i cÃ³ Ã­t nháº¥t 8 kÃ½ tá»±, bao gá»“m chá»¯ hoa, chá»¯ thÆ°á»ng vÃ  sá»‘.";
                return View(Users);
            }
            else if (Phone != null && !_Validation.ValidatePhoneNumber(Phone))
            {
                ViewBag.Error = "Sá»‘ Ä‘iá»‡n thoáº¡i khÃ´ng há»£p lá»‡.";
                return View(Users);

            }


            bool Result = _MyRegister.Register(Users.Username, Users.PhoneNumber, NewAccount, NewPassword);

            if (Result)
            {

                ViewBag.Status = "ÄÄƒng kÃ½ thÃ nh cÃ´ng";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Email Ä‘Ã£ Ä‘Æ°á»£c sá»­ dá»¥ng tá»« trÆ°á»›c ";
                return View(Users);
            }

        }

        [HttpGet]
        public ActionResult RegisterAccount()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Denied()
        {
            return View();
        }

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {

            var results = await HttpContext
             .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var email = results.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = results.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var avatar = results.Principal.FindFirst("avatar")?.Value;
            var user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {

                _dbContext.Users.Add(new User()
                {
                    Email = email,
                    FullName = name,
                    Role = "User",
                    PasswordHash = "**************",
                    Salt = "**************",
                    Username = name,
                    CreatedAt = DateTime.Now
                });

                _dbContext.SaveChanges();
                user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            }

            var claim = new List<Claim>
            {
               new Claim("IdUser",user.UserId.ToString()),
               new Claim(ClaimTypes.Email,email),
               new Claim(ClaimTypes.Role,user.Role),
               new  Claim("FullName", user.Username),
               new Claim("IdUser", user.UserId.ToString()),
               new Claim("MyAvatar",avatar),
              new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())

            };

            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var princip = new ClaimsPrincipal(identity);

            await SaveTokenSession(user.UserId);

            _Logger.LogInformation("Tocken lÃ  :"+HttpContext.Session.GetString("token"));


            await HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme, princip
              );

            return RedirectToAction("Index", "Home");

        }

        public ActionResult SignOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");   // action  - controller 
        }



        public async Task<ActionResult> ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPasswordProcess([FromBody] string email)
        {

            Console.WriteLine($"email cá»§a báº¡n lÃ  {email}");

            var result = await _MyRegister.ResetPassword(email);

            if (result)
            {
                return Ok(new { status = result });

            }
            else
            {
                return NotFound(new { status = result });
            }
        }

    }
    public class TokenTemporary
    {
        public string token { get; set; }

        public string timeExpire { get; set; }

        public DateTime timeCreate { get; set; }
    }
}

