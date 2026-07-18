using API_BookingHotel.Modules.WorkWithFIles;
using API_BookingHotel.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Mydata.Models;
using MyData.Models;


namespace API_BookingHotel.Modules.AmentityModules.AmentityServices
{
    public class AmentityServices : IAmenityServices
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IMyFiles _file;

        public AmentityServices(ManagermentHotelContext dbcontext, IMyFiles file)
        {
            _dbcontext = dbcontext;
            _file = file;
        }

        public async Task<bool> ChangeStatusAmenityAsync(int id)
        {
            var amentity = await _dbcontext.Amenities.FirstOrDefaultAsync(s => s.AmenityId == id);
            if (amentity != null)
            {
                if (amentity.status == "Active")
                {
                    amentity.status = "Hidden";
                }
                else
                {
                    amentity.status = "Active";
                }

            }
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateAmenityAsync(AmentityUpdate request)
        {
            string fileName = "";
            if (request != null)
            {

                if (request.UpdateImage != null)
                {
                    string path = Path.Combine("wwwroot", "ImagesAmentity");
                    fileName = await _file.SaveFiles(request.UpdateImage, path);
                }

                var item = new Amenity()
                {
                    Name = request.Name,
                    Description = request.Description,
                    UrlImage = fileName,
                    status = "Active"
                };

                _dbcontext.Amenities.Add(item);
                return await _dbcontext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteAmenityAsync(int id)
        {
            var item = await _dbcontext.Amenities.FirstOrDefaultAsync(a => a.AmenityId == id);
            if (item != null)
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesAmentity");

                string pathImage = Path.Combine(folderPath, item.UrlImage ?? "");

                if (File.Exists(pathImage))
                {
                    File.Delete(pathImage);
                }

                _dbcontext.Remove(item);
                return await _dbcontext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<AmentityUpdate>> GetAllAmenityAsync(string apihost)
        {
            var amenities = await _dbcontext.Amenities.Select(s => new AmentityUpdate()
            {
                AmenityId = s.AmenityId,
                Name = s.Name,
                Description = s.Description,
                Status = s.status ?? "Active",
                UrlImage = s.UrlImage.StartsWith("http") ? s.UrlImage : $"{apihost}/ImagesAmentity/{s.UrlImage}"
            }).ToListAsync();

            if (amenities == null || amenities.Count == 0)
            {
                return new List<AmentityUpdate>();
            }
            else
            {
                return amenities;
            }
        }

        public async Task<AmentityUpdate> GetAmenityByIdAsync(int id, string apihost)
        {
            var amenities = await _dbcontext.Amenities.Where(a => a.AmenityId == id).Select(s => new AmentityUpdate()
            {
                AmenityId = s.AmenityId,
                Name = s.Name,
                Status = s.status ?? "Active",
                Description = s.Description,
                UrlImage = s.UrlImage.StartsWith("http") ? s.UrlImage : $"{apihost}/ImagesAmentity/{s.UrlImage}"
            }).FirstOrDefaultAsync();


            if (amenities == null)
            {
                return new AmentityUpdate()
                {
                    AmenityId = 0,
                    Name = "",
                    Description = "",
                    UrlImage = ""
                };
            }

            return amenities;

        }

        public async Task<bool> UpdateAmenityAsync(AmentityUpdate request)
        {
            string NameFIle = "";
            if (request != null)
            {

                var amenity = await _dbcontext.Amenities.FirstOrDefaultAsync(a => a.AmenityId == request.AmenityId);

                string NameImageCurrent = amenity?.UrlImage ?? "";

                if (request.UpdateImage != null)
                {
                    string folderPaths = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesAmentity");

                    string pathImageCurrent = Path.Combine(folderPaths, NameImageCurrent);

                    if (File.Exists(pathImageCurrent))
                    {
                        File.Delete(pathImageCurrent);
                    }

                    string folderPath = Path.Combine("wwwroot", "ImagesAmentity");

                    NameFIle = await _file.SaveFiles(request.UpdateImage, folderPath);

                }

                if (amenity != null)
                {
                    amenity.Name = request.Name;
                    amenity.Description = request.Description;

                    if (!string.IsNullOrEmpty(NameFIle))
                    {
                        amenity.UrlImage = NameFIle;
                    }

                    return await _dbcontext.SaveChangesAsync() > 0;
                }
            }

            return false;
        }

    }
}
