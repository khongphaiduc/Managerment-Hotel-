
using API_BookingHotel.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Mydata.Models;


namespace API_BookingHotel.Modules.Rooms.RoomsService
{
    public class RoomViewDetail
    {
        private readonly ManagermentHotelContext _dbcontext;

        public RoomViewDetail(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // Allow user to view detail the room   
        public async Task<ViewRoomDetail?> ViewDetailRoomAsync(int roomID, string apiHost)
        {
            var room = await _dbcontext.Rooms
                .AsNoTracking()
                .Where(r => r.RoomId == roomID)
                .Select(r => new
                {
                    r.RoomId,
                    r.RoomTypeId,
                    RoomTypeName = r.RoomType.Name,
                    r.RoomNumber,
                    r.Floor,
                    r.Status,
                    r.Description,
                    r.PathImage,
                    Price = r.RoomType.Price,
                    MaxGuests = r.RoomType.MaxGuests,

                    Images = r.Images.Select(i => i.LinkImage).ToList(),
                    Amenities = r.RoomAmenities
                        .Where(a => a.Amenity.status == "Active")
                        .Select(a => new
                        {
                            a.Amenity.AmenityId,
                            a.Amenity.Name,
                            a.Amenity.Description,
                            a.Amenity.UrlImage
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (room == null) return null;

            return new ViewRoomDetail
            {
                RoomId = room.RoomId,
                RoomTypeId = room.RoomTypeId,
                NameType = room.RoomTypeName,
                RoomNumber = room.RoomNumber,
                Floor = (int)room.Floor,
                Status = room.Status,
                Description = room.Description,
                PathImage = room.PathImage,
                Price = room.Price,
                MaxGuests = room.MaxGuests.ToString(),

                ListPathImage = room.Images
                    .Select(i => i.StartsWith("http") ? i : $"{apiHost}/images/{i}")
                    .ToList(),

                ListAmenites = room.Amenities
                    .Select(a => new MyAmenity
                    {
                        AmenityId = a.AmenityId,
                        Name = a.Name,
                        Description = a.Description,
                        UrlImage = a.UrlImage.StartsWith("http")
                            ? a.UrlImage
                            : $"{apiHost}/ImagesAmentity/{a.UrlImage}"
                    }).ToList()
            };
        }

    }
}
