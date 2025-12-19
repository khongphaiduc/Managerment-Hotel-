

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



namespace Management_Hotel_2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ManagermentHotelContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL")));

            //--------------------------------------------------------------------------------
            // 2. Đăng ký Quartz
            builder.Services.AddQuartz(q =>
            {
                // Tạo job tự động
                var jobKey = new JobKey("RefreshStatusRoomJob", "group1");           //JobKey là mã định danh (ID) của job

                q.AddJob<RefreshStatusRoom>(opts => opts.WithIdentity(jobKey));    //  đăng ký job


                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("RefreshStatusRoomTrigger", "group1")
                 .WithCronSchedule("0 0 22 * * ?")); // hẹn 22h chạy mỗi ngày





                // thống báo check-in
                var jobKeyInformCheckIn = new JobKey("InformPassngerDateCheckInJob", "group1");
                q.AddJob<InformPassngerDateCheckIn>(opts => opts.WithIdentity(jobKeyInformCheckIn));
                q.AddTrigger(opts => opts
                    .ForJob(jobKeyInformCheckIn)
                    .WithIdentity("InformPassngerDateCheckInTrigger", "group1")
                 .WithCronSchedule("0 0 7 * * ?")); // hẹn 7h chạy mỗi ngày




                // thông báo trả phòng 
                var jobKeyInformCheckOut = new JobKey("InformPassengerDateCheckOutJob", "group1");
                q.AddJob<InformPassengerDateCheckOut>(opts => opts.WithIdentity(jobKeyInformCheckOut));
                q.AddTrigger(opts => opts
                    .ForJob(jobKeyInformCheckOut)
                    .WithIdentity("InformPassengerDateCheckOutTrigger", "group1")
                 .WithCronSchedule("0 0 9 * * ?")); // hẹn 9h chạy mỗi ngày



                // tính toán trả phòng muộn chạy đúng 12 giờ trưa mỗi ngày 
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

            // 3. Thêm QuartzHostedService để Quartz tự chạy  (tự động run khi app mở) 
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            //--------------------------------------------------------------------------------

            // AddAuthentication là Bật hệ thống xác thực cho ứng dụng
            builder.Services
            .AddAuthentication(option =>
            {
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //option.DefaultScheme : scheme mặc định dùng cho tất cả các hành động liên quan đến authentication nếu k set riêng các scheme khác

                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // option.DefaultSignInScheme : dùng để chỉ định nơi lưu chữ thông tin của user khi đăng nhập thành công (có thể là Cookie) , dùng khi gọi SignInAsync() không truyền scheme

                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // option.DefaultAuthenticateScheme : xác định  nơi sẽ lấy thông tin để xác thực user, dùng khi Authorize hoặc HttpContext.User
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Authen/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Authen/Denied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            }).AddGoogle("Google", options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:GoogleID").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:GoogleSecret").Value;
                options.ClaimActions.MapJsonKey("avatar", "picture", "url");         // Lấy giá trị picture trong JSON của Google và lưu nó vào Claims với tên ‘avatar’. 
            });





            builder.Services.AddSession();  //  đăng ký dịch vụ Session trong ứng dụng ASP.NET Core.

            // CookieAuthenticationDefaults trong ASP.NET Core thực chất là một class chứa các hằng  số (constant) được Microsoft định nghĩa sẵn để dùng cho cấu hình Cookie Authentication.

            // bản chất nó là builder.Services.AddAuthentication("Cookies").AddCookie(); nhưng khuyếch nghị dùng (CookieAuthenticationDefaults.AuthenticationScheme) để tránh lỗi chính tả và dễ dàng thay đổi trong tương lai.



            //Transient : mỗi lần inject sẽ là  1 instance mới  được tạo ra
            // Scoped   : 1 request sẽ inject duy nhất 1 instance 
            //Singleton : 1 instance sẽ tồn tại đến hết vòng đời của ứng dụng  (tất cả các lần inject đều dùng chung 1 instance)
            builder.Services.AddSingleton<EmailProducer>();
          

            builder.Services.AddTransient<INotifications, Email>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddSingleton<IEncoding, MyEncoding>();
            builder.Services.AddScoped<RegisterAccount>();
            builder.Services.AddScoped<ValidationAuthen>();
            builder.Services.AddScoped<Login>();
            builder.Services.AddTransient<GenarateTokenHotel>();

            builder.Services.AddHttpClient(); // Thêm HttpClient để gọi API bên ngoài
            builder.Services.AddHttpContextAccessor();  // Thêm HttpContextAccessor để truy cập HttpContext trong các dịch vụ

            builder.Services.AddTransient<IRoomService, RoomSerices>();
            builder.Services.AddTransient<IManagementRoom, FilterRooms>();
            builder.Services.AddTransient<IManagementBooking, ManagementBooking>();

            builder.Services.AddTransient<IGanarateQRCode, QRCodeBookingDetail>();

            builder.Services.AddTransient<IReceptionService, ReceptionService>();

            builder.Services.AddTransient<IOrder, ViewOrder>();

            builder.Services.AddTransient<IAdminManagement, AdminManagement>();
            builder.Services.AddTransient<IMyFiles, MyFiles>();
            builder.Services.AddTransient<IAdminMPassengers, AdminMPassengers>();
            builder.Services.AddTransient<IGameBlackRed, GameBlackRed>();
            ////-----
            //builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();
            ////-----

         
   



            // Đăng ký PayOSClient
            builder.Services.AddSingleton<PayOSClient>(sp =>
            {
                return new PayOSClient(new PayOSOptions
                {
                    ClientId = builder.Configuration["PayOS:ClientId"],
                    ApiKey = builder.Configuration["PayOS:ApiKey"],
                    ChecksumKey = builder.Configuration["PayOS:ChecksumKey"]

                });
            });




            builder.Services.AddSignalR();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.MapHub<CoinHub>("/coinhub");
            app.MapHub<TaiXiuGames>("/taixiugame");
            app.MapHub<NotificationSystem>("/notificationsystem");

            app.UseSession();             // Kích hoạt Session trong ứng dụng
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Intro}/{id?}");
            app.Run();
        }
    }
}


