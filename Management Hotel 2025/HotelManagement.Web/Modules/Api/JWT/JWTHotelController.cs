using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_BookingHotel.Modules.JWT
{
    [Route("hotel")]
    [ApiController]
    public class JWTHotelController : ControllerBase
    {
        private readonly IConfiguration _config;

        public JWTHotelController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost("token")]
        public IActionResult Get(UserTemporary user)
        {

            int time = 5;

            var listClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: listClaim,
                    expires: DateTime.UtcNow.AddHours(time),
                    signingCredentials: creds);


            if (token != null)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    timeExpire = $"{time} hours",
                    timeCreate = DateTime.Now
                });
            }
            else
            {
                return BadRequest("get token failure");
            }
        }

    }

    public class UserTemporary
    {
        public string Name { get; set; }

        public string Role { get; set; }
    }
}
