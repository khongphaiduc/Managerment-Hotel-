using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Management_Hotel_2025.Serives.GenarateToken
{
    public class GenarateTokenHotel
    {
        private readonly IConfiguration _Configuration;

        public GenarateTokenHotel(IConfiguration configuration)
        {
            _Configuration = configuration;
           
        }


        public string GetGenarateTokenHotel(int IdUser,int time)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, IdUser.ToString()),
                new Claim(ClaimTypes.Role, "Collaborator"),

            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_Configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                   issuer: _Configuration["Jwt:Issuer"],
                   audience: _Configuration["Jwt:Audience"],
                   claims: claims,
                   expires: DateTime.Now.AddMinutes(time),
                   signingCredentials: creds);



            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
