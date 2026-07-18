using System.ComponentModel.DataAnnotations;

namespace Management_Hotel_2025.ViewModel
{
    public class RoomTypeViewModel
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class AmenityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public class ImageViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class AdJustRoom
    {

        public int RoomId { get; set; }

        [Display(Name = "Loại phòng")]
        public int RoomTypeId { get; set; }

        [Display(Name = "Số phòng")]
        public string RoomNumber { get; set; }

        [Display(Name = "Tầng")]
        public int Floor { get; set; }

        [Display(Name = "Giá mỗi đêm")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }



        public List<RoomTypeViewModel> AllRoomTypes { get; set; } = new List<RoomTypeViewModel>();

        public List<AmenityViewModel> AllAvailableAmenities { get; set; } = new List<AmenityViewModel>();
        public List<AmenityViewModel> CurrentAmenities { get; set; } = new List<AmenityViewModel>();

        public List<int> DeletedAmenity { get; set; } = new List<int>();


        public List<int> NewAmenities { get; set; } = new List<int>();

        public List<ImageViewModel> CurrentImages { get; set; } = new List<ImageViewModel>();

        public List<int> DeletedImageIds { get; set; } = new List<int>();

        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();


        public IFormFile? AvatarRoom { get; set; }

         public string? AvatarRoomRecive { get; set; }



        //------------------------------------------------------------------------------------------------


    }
}
