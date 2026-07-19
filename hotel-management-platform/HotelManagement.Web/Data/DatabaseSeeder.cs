using Management_Hotel_2025.Modules.AuthenSerive;
using Management_Hotel_2025.Serives.AuthenSerive;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using MyData.Models;

namespace Management_Hotel_2025.Data;

public static class DatabaseSeeder
{
    public static void Seed(ManagermentHotelContext db)
    {
        var encoding = new MyEncoding();

        SeedUsers(db, encoding);
        SeedRoomTypesAndRooms(db);
        SeedAmenities(db);
        SeedServices(db);

        db.SaveChanges();
    }

    private static void SeedUsers(ManagermentHotelContext db, IEncoding encoding)
    {
        if (db.Users.Any())
        {
            return;
        }

        AddUser(db, encoding, 1, "admin", "admin123", "admin@novastay.local", "System Admin", "Admin");
        AddUser(db, encoding, 2, "staff", "staff123", "staff@novastay.local", "Hotel Staff", "Staff");
        AddUser(db, encoding, 3, "customer", "customer123", "customer@novastay.local", "Demo Customer", "Customer");
    }

    private static void AddUser(
        ManagermentHotelContext db,
        IEncoding encoding,
        int id,
        string username,
        string password,
        string email,
        string fullName,
        string role)
    {
        const string salt = "NovaStaySeedSalt";

        db.Users.Add(new User
        {
            UserId = id,
            Username = username,
            PasswordHash = encoding.HashPassword(password, salt),
            Email = email,
            FullName = fullName,
            PhoneNumber = "0900000000",
            Role = role,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    private static void SeedRoomTypesAndRooms(ManagermentHotelContext db)
    {
        if (!db.RoomTypes.Any())
        {
            db.RoomTypes.AddRange(
                new RoomType
                {
                    RoomTypeId = 1,
                    Name = "Standard Room",
                    Description = "Phòng tiêu chuẩn phù hợp cho 1 đến 2 khách.",
                    Price = 650000,
                    MaxGuests = 2
                },
                new RoomType
                {
                    RoomTypeId = 2,
                    Name = "Deluxe Room",
                    Description = "Phòng deluxe rộng rãi, đầy đủ tiện nghi.",
                    Price = 950000,
                    MaxGuests = 3
                },
                new RoomType
                {
                    RoomTypeId = 3,
                    Name = "Family Suite",
                    Description = "Phòng suite dành cho gia đình hoặc nhóm nhỏ.",
                    Price = 1500000,
                    MaxGuests = 4
                });
        }

        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(
                CreateRoom(1, 1, "101", 1, "Phòng Standard tầng 1", 650000),
                CreateRoom(2, 1, "102", 1, "Phòng Standard tầng 1", 650000),
                CreateRoom(3, 2, "201", 2, "Phòng Deluxe tầng 2", 950000),
                CreateRoom(4, 2, "202", 2, "Phòng Deluxe tầng 2", 950000),
                CreateRoom(5, 3, "301", 3, "Phòng Family Suite tầng 3", 1500000));
        }
    }

    private static Room CreateRoom(
        int id,
        int roomTypeId,
        string roomNumber,
        int floor,
        string description,
        decimal price)
    {
        return new Room
        {
            RoomId = id,
            RoomTypeId = roomTypeId,
            RoomNumber = roomNumber,
            Floor = floor,
            Status = "available",
            Description = description,
            PathImage = "/images/rooms/default-room.jpg",
            PricePrivate = price
        };
    }

    private static void SeedAmenities(ManagermentHotelContext db)
    {
        if (!db.Amenities.Any())
        {
            db.Amenities.AddRange(
                new Amenity { AmenityId = 1, Name = "Wi-Fi", Description = "Wi-Fi miễn phí", status = "Active" },
                new Amenity { AmenityId = 2, Name = "Điều hòa", Description = "Điều hòa nhiệt độ", status = "Active" },
                new Amenity { AmenityId = 3, Name = "TV màn hình phẳng", Description = "Smart TV trong phòng", status = "Active" },
                new Amenity { AmenityId = 4, Name = "Bữa sáng", Description = "Bữa sáng miễn phí", status = "Active" });
        }

        if (!db.RoomAmenities.Any())
        {
            db.RoomAmenities.AddRange(
                new RoomAmenity { IDRoomAmenity = 1, RoomId = 1, AmenityId = 1, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 2, RoomId = 1, AmenityId = 2, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 3, RoomId = 1, AmenityId = 3, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 4, RoomId = 2, AmenityId = 1, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 5, RoomId = 2, AmenityId = 2, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 6, RoomId = 3, AmenityId = 1, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 7, RoomId = 3, AmenityId = 2, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 8, RoomId = 3, AmenityId = 3, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 9, RoomId = 4, AmenityId = 4, Quanlity = 1 },
                new RoomAmenity { IDRoomAmenity = 10, RoomId = 5, AmenityId = 4, Quanlity = 1 });
        }
    }

    private static void SeedServices(ManagermentHotelContext db)
    {
        if (db.HotelServices.Any())
        {
            return;
        }

        db.HotelServices.AddRange(
            new Services
            {
                ServiceId = 1,
                ServiceName = "Airport pickup",
                Description = "Đưa đón sân bay theo lịch hẹn.",
                Price = 250000,
                Discount = 0,
                LastUpdate = DateTime.UtcNow
            },
            new Services
            {
                ServiceId = 2,
                ServiceName = "Extra bed",
                Description = "Thêm một giường phụ trong phòng.",
                Price = 150000,
                Discount = 0,
                LastUpdate = DateTime.UtcNow
            },
            new Services
            {
                ServiceId = 3,
                ServiceName = "Laundry",
                Description = "Dịch vụ giặt ủi trong ngày.",
                Price = 100000,
                Discount = 0,
                LastUpdate = DateTime.UtcNow
            });
    }
}
