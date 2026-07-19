
using Management_Hotel_2025.Modules.AuthenSerive;
using Management_Hotel_2025.Modules.ManagementQRCode;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.Rooms.ManagementRoom;
using Management_Hotel_2025.Modules.Rooms.RoomService;
using Management_Hotel_2025.Modules.Secheduler;
using Management_Hotel_2025.Serives.AuthenSerive;
using Management_Hotel_2025.Serives.GenarateToken;
using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using Quartz;
using PayOS;
using Management_Hotel_2025.Modules.Rooms.RoleAdmin.AdminServices;
using Management_Hotel_2025.Modules.WorkFile;
using Management_Hotel_2025.Modules.AdminMPassengers.MPassengersServices;
using Management_Hotel_2025.Modules.Payment.PayOSPayments;
using Management_Hotel_2025.Modules.SignalRModels;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using StackExchange.Redis;
using Management_Hotel_2025.Modules.RedisServices;
using Management_Hotel_2025.Modules.RabbitMQConsumer;
using Management_Hotel_2025.Data;
using MyData.Configuration;
using API_BookingHotel.MiddlewareCustom;
using API_BookingHotel.Modules.AmentityModules.AmentityServices;
using API_BookingHotel.Modules.Invoice.MInvoiceServices;
using API_BookingHotel.Modules.MPassengers.AdminPassengersSerives;
using API_BookingHotel.Modules.Statistics.StatisticsServices;
using API_BookingHotel.Modules.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;



namespace Management_Hotel_2025
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotEnvLoader.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            var sqlConnection = builder.Configuration.GetConnectionString("SQL")
                ?? builder.Configuration["ConnectionStrings:SQL"]
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__SQL");

            if (string.IsNullOrWhiteSpace(sqlConnection))
            {
                throw new InvalidOperationException(
                    "SQL connection string was not found. Configure ConnectionStrings__SQL in the .env file.");
            }

            builder.Services.AddDbContext<ManagermentHotelContext>(options =>
                options.UseSqlServer(sqlConnection));

            builder.Services.AddMemoryCache();

            var redisEnabled = bool.TryParse(builder.Configuration["Redis:Enabled"], out var useRedis) && useRedis;
            if (redisEnabled)
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration["Redis:Connection"] ?? "localhost:7000";
                    options.InstanceName = "HotelCache_";
                });
            }
            else
            {
                builder.Services.AddDistributedMemoryCache();
            }

            //--------------------------------------------------------------------------------
            builder.Services.AddQuartz(q =>
            {
                var jobKey = new JobKey("RefreshStatusRoomJob", "group1");
                q.AddJob<RefreshStatusRoom>(opts => opts.WithIdentity(jobKey));



                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("RefreshStatusRoomTrigger", "group1")
                    .WithCronSchedule("0 0 22 * * ?"));





                var jobKeyInformCheckIn = new JobKey("InformPassngerDateCheckInJob", "group1");
                q.AddJob<InformPassngerDateCheckIn>(opts => opts.WithIdentity(jobKeyInformCheckIn));
                q.AddTrigger(opts => opts
                    .ForJob(jobKeyInformCheckIn)
                    .WithIdentity("InformPassngerDateCheckInTrigger", "group1")
                    .WithCronSchedule("0 0 7 * * ?"));




                var jobKeyInformCheckOut = new JobKey("InformPassengerDateCheckOutJob", "group1");
                q.AddJob<InformPassengerDateCheckOut>(opts => opts.WithIdentity(jobKeyInformCheckOut));
                q.AddTrigger(opts => opts
                    .ForJob(jobKeyInformCheckOut)
                    .WithIdentity("InformPassengerDateCheckOutTrigger", "group1")
                    .WithCronSchedule("0 0 9 * * ?"));



                var jobKeyLateCheckOutCalculator = new JobKey("UpdateRoomStatusJob", "group1");
                q.AddJob<LateCheckOutCalculator>(opts => opts.WithIdentity(jobKeyLateCheckOutCalculator));

                q.AddTrigger(opts => opts
                    .ForJob(jobKeyLateCheckOutCalculator)
                    .WithIdentity("UpdateRoomStatusTrigger", "group1")
                 .WithCronSchedule("0 0 12 * * ?"));

                q.AddTrigger(s => s.ForJob(jobKeyLateCheckOutCalculator)
                .WithIdentity("UpdateRoomStatusTrigger_Startup", "group1")
                .StartNow());



            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            


            builder.Services
            .AddAuthentication(option =>
            {
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Authen/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Authen/Denied";
            }).AddGoogle("Google", options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:GoogleID").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:GoogleSecret").Value;
                options.ClaimActions.MapJsonKey("avatar", "picture", "url");
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
                };
            });





            builder.Services.AddSession();





            builder.Services.AddSingleton<EmailProducer>();
            builder.Services.AddHostedService<EmailConsumer>();
            builder.Services.AddSingleton<IApiJwtTokenService, ApiJwtTokenService>();

            // API modules hosted by the MVC application.
            builder.Services.AddTransient<API_BookingHotel.Modules.Rooms.RoomsService.RoomViewDetail>();
            builder.Services.AddTransient<API_BookingHotel.Modules.Rooms.RoomsService.IRoomService, API_BookingHotel.Modules.Rooms.RoomsService.RoomSearchWithPagination>();
            builder.Services.AddTransient<API_BookingHotel.Modules.WorkWithFIles.IMyFiles, API_BookingHotel.Modules.WorkWithFIles.MyFiles>();
            builder.Services.AddTransient<API_BookingHotel.Modules.Rooms.RoomsService.IEditableRoom, API_BookingHotel.Modules.Rooms.RoomsService.EditRoom>();
            builder.Services.AddTransient<API_BookingHotel.Modules.AmentityModules.AmentityServices.IAmenityServices, API_BookingHotel.Modules.AmentityModules.AmentityServices.AmentityServices>();
            builder.Services.AddTransient<API_BookingHotel.Modules.MPassengers.AdminPassengersSerives.IPassengers, API_BookingHotel.Modules.MPassengers.AdminPassengersSerives.Passengers>();
            builder.Services.AddTransient<API_BookingHotel.Modules.Invoice.MInvoiceServices.IInvoiceServices, API_BookingHotel.Modules.Invoice.MInvoiceServices.InvoiceService>();
            builder.Services.AddTransient<API_BookingHotel.Modules.Statistics.StatisticsServices.IStatisticsServices, API_BookingHotel.Modules.Statistics.StatisticsServices.StatisticsServices>();


            builder.Services.AddTransient<INotifications, Email>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddSingleton<IEncoding, MyEncoding>();
            builder.Services.AddScoped<RegisterAccount>();
            builder.Services.AddScoped<ValidationAuthen>();
            builder.Services.AddScoped<Login>();
            builder.Services.AddTransient<GenarateTokenHotel>();


            builder.Services.AddTransient<IRoomService, RoomSerices>();
            builder.Services.AddTransient<IManagementRoom, FilterRooms>();
            builder.Services.AddTransient<IManagementBooking, ManagementBooking>();

            builder.Services.AddTransient<IGanarateQRCode, QRCodeBookingDetail>();

            builder.Services.AddTransient<IReceptionService, ReceptionService>();

            builder.Services.AddTransient<IOrder, ViewOrder>();

            builder.Services.AddTransient<IAdminManagement, AdminManagement>();
            builder.Services.AddTransient<IMyFiles, MyFiles>();
            builder.Services.AddTransient<IAdminMPassengers, AdminMPassengers>();



            if (redisEnabled)
            {
                builder.Services.AddSingleton<IRedisLockService, RedisLockService>();
            }
            else
            {
                builder.Services.AddSingleton<IRedisLockService, InMemoryRedisLockService>();
            }



            builder.Services.AddSingleton<PayOSClient>(sp =>
            {
                return new PayOSClient(new PayOSOptions
                {
                    ClientId = builder.Configuration["PayOS:ClientId"],
                    ApiKey = builder.Configuration["PayOS:ApiKey"],
                    ChecksumKey = builder.Configuration["PayOS:ChecksumKey"]

                });
            });






            if (redisEnabled)
            {
                builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(
                        builder.Configuration["Redis:Connection"] ?? "localhost:7000"));
            }




            builder.Services.AddSignalR();


            var app = builder.Build();

            await DatabaseInitializer.InitializeAsync(app.Services, app.Logger);

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                
                app.UseHsts();
            }

        //E:\Hotel PHAMTRUNGDUC\hotel - management - platform\HotelManagement.sln

            app.MapHub<NotificationSystem>("/notificationsystem");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Intro}/{id?}");
            await app.RunAsync();
        }
    }
}


