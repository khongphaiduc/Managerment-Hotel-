using Management_Hotel_2025.Modules.AuthenSerive;
using Microsoft.EntityFrameworkCore;
using MyData.Models;
using Mydata.Models;

namespace Management_Hotel_2025.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ManagermentHotelContext>();
        var encoder = scope.ServiceProvider.GetRequiredService<IEncoding>();

        var databaseExists = await db.Database.CanConnectAsync();
        logger.LogInformation("Hotel database connection check: {DatabaseExists}", databaseExists);

        // MigrateAsync creates the database when it does not exist, then applies
        // all migrations needed to bring it to the current schema.
        await db.Database.MigrateAsync();

        // Keep startup idempotent. Existing databases and existing user data are
        // left untouched; the sample data is inserted only for an empty database.
        if (await db.Users.AnyAsync())
        {
            logger.LogInformation("Hotel database already contains data. Sample seed skipped.");
            return;
        }

        var admin = CreateUser(
            encoder,
            username: "admin",
            email: "admin@trungduchotel.com",
            fullName: "Hotel Administrator",
            phone: "0900000001",
            role: "Admin",
            password: "Admin@123");

        var receptionist = CreateUser(
            encoder,
            username: "receptionist",
            email: "receptionist@trungduchotel.com",
            fullName: "Hotel Receptionist",
            phone: "0900000002",
            role: "Staff",
            password: "Staff@123");

        var customer = CreateUser(
            encoder,
            username: "customer",
            email: "customer@example.com",
            fullName: "Sample Customer",
            phone: "0900000003",
            role: "User",
            password: "User@123");

        // Save each aggregate in dependency order. This keeps SQL Server-generated
        // identity values out of later inserts and makes the seed easier to recover
        // from if the application is stopped during startup.
        db.Users.AddRange(admin, receptionist, customer);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var standard = new RoomType
        {
            Name = "Standard Room",
            Description = "Comfortable room for individual travellers or couples.",
            Price = 650000,
            MaxGuests = 2
        };
        var deluxe = new RoomType
        {
            Name = "Deluxe Room",
            Description = "Spacious room with upgraded amenities.",
            Price = 950000,
            MaxGuests = 3
        };
        var family = new RoomType
        {
            Name = "Family Suite",
            Description = "Large suite suitable for families.",
            Price = 1500000,
            MaxGuests = 5
        };
        db.RoomTypes.AddRange(standard, deluxe, family);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var rooms = new[]
        {
            new Room { RoomTypeId = standard.RoomTypeId, RoomNumber = "101", Floor = 1, Status = "Active", Description = "Standard room on the first floor.", PathImage = "", PricePrivate = 650000 },
            new Room { RoomTypeId = standard.RoomTypeId, RoomNumber = "102", Floor = 1, Status = "Active", Description = "Quiet standard room.", PathImage = "", PricePrivate = 650000 },
            new Room { RoomTypeId = deluxe.RoomTypeId, RoomNumber = "201", Floor = 2, Status = "Active", Description = "Deluxe room with city view.", PathImage = "", PricePrivate = 950000 },
            new Room { RoomTypeId = family.RoomTypeId, RoomNumber = "301", Floor = 3, Status = "Active", Description = "Family suite with extra sleeping space.", PathImage = "", PricePrivate = 1500000 }
        };
        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var wifi = new Amenity { Name = "Free Wi-Fi", Description = "High-speed wireless internet.", status = "Active", UrlImage = "" };
        var breakfast = new Amenity { Name = "Breakfast", Description = "Breakfast included at the hotel restaurant.", status = "Active", UrlImage = "" };
        var parking = new Amenity { Name = "Free Parking", Description = "Complimentary parking for hotel guests.", status = "Active", UrlImage = "" };
        db.Amenities.AddRange(wifi, breakfast, parking);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        db.HotelServices.AddRange(
            new Services { ServiceName = "Airport Transfer", Description = "One-way airport transfer.", Price = 250000, Discount = 0 },
            new Services { ServiceName = "Laundry Service", Description = "Same-day laundry service.", Price = 100000, Discount = 0 },
            new Services { ServiceName = "Extra Breakfast", Description = "Breakfast for one guest.", Price = 120000, Discount = 0 });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        db.RoomAmenities.AddRange(
            new RoomAmenity { RoomId = rooms[0].RoomId, AmenityId = wifi.AmenityId, Quanlity = 1 },
            new RoomAmenity { RoomId = rooms[0].RoomId, AmenityId = parking.AmenityId, Quanlity = 1 },
            new RoomAmenity { RoomId = rooms[2].RoomId, AmenityId = wifi.AmenityId, Quanlity = 1 },
            new RoomAmenity { RoomId = rooms[2].RoomId, AmenityId = breakfast.AmenityId, Quanlity = 1 },
            new RoomAmenity { RoomId = rooms[3].RoomId, AmenityId = wifi.AmenityId, Quanlity = 1 },
            new RoomAmenity { RoomId = rooms[3].RoomId, AmenityId = breakfast.AmenityId, Quanlity = 1 });

        await db.SaveChangesAsync();
        logger.LogInformation("Sample hotel data seeded successfully. Sample login: customer@example.com / User@123");
    }

    private static User CreateUser(
        IEncoding encoder,
        string username,
        string email,
        string fullName,
        string phone,
        string role,
        string password)
    {
        var salt = encoder.GenerateSalt();
        return new User
        {
            Username = username,
            Email = email,
            FullName = fullName,
            PhoneNumber = phone,
            Role = role,
            Salt = salt,
            PasswordHash = encoder.HashPassword(password, salt),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
}
