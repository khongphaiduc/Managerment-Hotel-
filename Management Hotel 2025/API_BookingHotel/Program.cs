
using Microsoft.Extensions.Caching.Memory;
using API_BookingHotel.Modules.Rooms.RoomsService;
using Mydata.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MyData.Models;
using API_BookingHotel.Modules.WorkWithFIles;
using API_BookingHotel.Modules.AmentityModules.AmentityServices;
using API_BookingHotel.Modules.MPassengers.AdminPassengersSerives;
using API_BookingHotel.Modules.Invoice.MInvoiceServices;
using API_BookingHotel.Modules.Statistics.StatisticsServices;

namespace API_BookingHotel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IdentityModelEventSource.ShowPII = true;
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ManagermentHotelContext>
                (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddMemoryCache();  // đăng ký dịch vụ cache


            builder.Services.AddAuthentication(
               option => option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme
            ).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,    // người phát hành token
                    ValidateAudience = true,   //Audience thường là ứng dụng hoặc client được phép dùng token này.
                    ValidateLifetime = true,   // thời gian sử dụng của token
                    ValidateIssuerSigningKey = true,  // chữ ý (sign) của token
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };

            });



            builder.Services.AddTransient<RoomViewDetail>();
            builder.Services.AddTransient<IRoomService, RoomSearchWithPagination>();
            builder.Services.AddTransient<IMyFiles, MyFiles>();
            builder.Services.AddTransient<IEditableRoom, EditRoom>();
            builder.Services.AddTransient<IAmenityServices, AmentityServices>();
            builder.Services.AddTransient<IPassengers, Passengers>();
            builder.Services.AddTransient<IInvoiceServices, InvoiceService>();
            builder.Services.AddTransient<IStatisticsServices, StatisticsServices>();
            builder.Services.AddControllers();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();   // cho phép truy cập ảnh tĩnh trong project wwwroot


            app.UseHttpsRedirection();

            app.UseAuthentication();   // xác thực xem mày là ai 
            app.UseAuthorization();    // xác thực xem mày có quyền làm gì


            app.MapControllers();

            app.Run();
        }
    }
}
