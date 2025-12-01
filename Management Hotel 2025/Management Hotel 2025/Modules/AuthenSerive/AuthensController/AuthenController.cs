

using Management_Hotel_2025.Serives.AuthenSerive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Mydata.Models;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.AuthenSerive.AuthensController
{
    public class AuthenController : Controller
    {
        private readonly ManagermentHotelContext _dbContext;
        private readonly RegisterAccount _MyRegister;
        private readonly ValidationAuthen _Validation;
        private readonly Login _Login;
        private readonly ILogger<AuthenController> _Logger;

        public AuthenController(ManagermentHotelContext dbcontext, RegisterAccount MyRegister, ValidationAuthen Validation, Login login, ILogger<AuthenController> logger)
        {
            _dbContext = dbcontext;
            _MyRegister = MyRegister;
            _Validation = Validation;
            _Login = login;
            _Logger = logger;
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] User users)
        {
            string email = users.Email;
            string password = users.PasswordHash;

            // 1. Kiểm tra đăng nhập
            var result = _Login.MyLogin(email, password);
            if (!result)
            {
                return Json(new { success = false });
            }

            var IdUser = (from a in _dbContext.Users
                          where a.Email == users.Email
                          select a.UserId).FirstOrDefault();


            // 2. Lấy thông tin user từ DB (bao gồm role)
            var userFromDb = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (userFromDb == null)
            {
                return Json(new { success = false });
            }

            // 3. Tạo claims
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, userFromDb.Email),
               new Claim(ClaimTypes.Role, userFromDb.Role),   // Role từ DB
               new Claim("FullName", userFromDb.FullName), // Thêm claim FullName nếu cần
               new Claim("IdUser", IdUser.ToString()),
               new Claim(ClaimTypes.NameIdentifier, IdUser.ToString())  // sử dụng để map với SignalR
            };

            /* Trong bảo mật, Claim là một thông tin về người dùng mà hệ thống xác nhận là đúng sau khi người đó đăng nhập.
             Mỗi Claim là một cặp(key, value).*/


            /*     Giả sử bạn vào tòa nhà công ty, lễ tân đưa cho bạn thẻ khách ghi:

             Tên: Phạm Trung Đức

             Vai trò: Khách VIP

             Bộ phận: IT

              Cái thẻ đó chính là Identity,

              Còn từng dòng thông tin trên thẻ chính là Claim.
           */



            // 4. Tạo identity & principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            /*ClaimsIdentity:
            Đây là đối tượng danh tính của user.
            Nó chứa toàn bộ claims bạn vừa tạo(tên, role, …) +thông tin scheme đang dùng.*/
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            /*    ClaimsPrincipal:
                    Đại diện cho toàn bộ người dùng hiện tại(principal = “người chính”).
                    Nó có thể chứa nhiều identity(ví dụ: 1 identity từ Facebook, 1 từ Google, 1 từ DB nội bộ), nhưng ở đây ta chỉ có 1.

                  claimsIdentity: Identity bạn vừa tạo ở trên, được thêm vào principal.

                hiểu: Nếu Identity là “thẻ nhân viên” của bạn,thì Principal là “bạn” — người đang cầm thẻ đó.*/

            // 5. Đăng nhập (lưu cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);

            /*   Mục đích: Tạo phiên đăng nhập(login session) cho người dùng và lưu thông tin vào cookie trên trình duyệt.

            Sau khi gọi lệnh này, ASP.NET Core sẽ:

            - Lấy các thông tin của người dùng trong claimsPrincipal(tên, email, role, v.v.).

            - Đóng gói lại và mã hóa thành một cookie.

            - Gửi cookie đó về trình duyệt.

            - Mỗi lần người dùng gửi request mới, cookie này sẽ được gửi kèm để xác thực.
            */



            // 6. Trả JSON
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
                ViewBag.Error = "Số điện thoại đã được sử dụng, vui lòng nhập số khác.";
                return View(Users);
            }
            else if (!_Validation.ValidateEmail(NewAccount))
            {
                ViewBag.Error = "Email không hợp lệ.";
                return View(Users);
            }
            else if (NewPassword != null && !_Validation.ValidatePassword(NewPassword))
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số.";
                return View(Users);
            }
            else if (Phone != null && !_Validation.ValidatePhoneNumber(Phone))
            {
                ViewBag.Error = "Số điện thoại không hợp lệ.";
                return View(Users);

            }


            bool Result = _MyRegister.Register(Users.Username, Users.PhoneNumber, NewAccount, NewPassword);

            if (Result)
            {

                ViewBag.Status = "Đăng ký thành công";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Email đã được sử dụng từ trước ";
                return View(Users);
            }

            // đăng ký thành công chuyển đến controller đăng nhập
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

        // đăng nhập bằng gg 
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,    //  Asp sẽ lấy toàn bộ các thong tin cấu hình bên Program.cs và chuyền để trang đăng nhập của gg 
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")  // nếu đăng nhập thành công thì sẽ chuyển đến url này 
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

            if (user == null)   //  nếu bằng user bằng null có nghĩa là thằng này chưa từng đăng nhập
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
                user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();  // gắn lại giá trị cho user
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

            // đăng nhập
            await HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme, princip
              );

            return RedirectToAction("Index", "Home");

        }

        public ActionResult SignOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Xóa cookie đăng nhập
            // chuyển hướng về home
            return RedirectToAction("Index", "Home");   // action  - controller 
        }



        public async Task<ActionResult> ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPasswordProcess([FromBody] string email)
        {

            Console.WriteLine($"email của bạn là {email}");

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
}

