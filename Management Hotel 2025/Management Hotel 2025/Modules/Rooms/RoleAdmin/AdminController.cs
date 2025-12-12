using Management_Hotel_2025.Modules.Rooms.RoleAdmin.AdminServices;
using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Rooms.RoleAdmin
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminManagement _iadmin;
        private string _apiBaseUrl;
        public AdminController(IAdminManagement iadmin, ILogger<AdminController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _iadmin = iadmin;
            _apiBaseUrl = configuration["ApiHotel:AdminEditRoom"];
        }


        [AllowAnonymous]
        [HttpPut("hide/{idroom}")]
        public async Task<IActionResult> HideRoom(int idroom)
        {
            var result = await _iadmin.HideRoom(idroom);

            if (result)
            {
                return Ok(new { success = true, message = "Đã ẩn phòng thành công!" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Ẩn phòng thất bại!" });
            }
        }




        // xem quanlý phòng và search
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("room")]
        public IActionResult AdminManagementRoom(int? floor, string? status, string? key)
        {

             if(!string.IsNullOrEmpty(key))
            {
                key = key.Trim();
            }

            ViewBag.floor = floor;
            ViewBag.status = status;
            ViewBag.key = key;

            var ListFloor = _iadmin.NumberOfFloor();
            var StatusRoom = _iadmin.StatusRoom();

            List<ViewRoomModel> listRoom = new List<ViewRoomModel>();

            if (!floor.HasValue && string.IsNullOrEmpty(status) && string.IsNullOrEmpty(key))
            {

                listRoom = _iadmin.ViewTypeRoom();
            }
            else
            {

                listRoom = _iadmin.SearchRoom(floor, status, key);
            }

            var item = new AdminManagementRoom()
            {
                ListFloor = ListFloor,
                ListStatusRoom = StatusRoom,
                ListViewRooms = listRoom

            };
            return View(item);
        }


        //xem trạng thái phòng của ngày
        [Authorize(Roles = "Admin")]
        [Route("serveralroom")]
        public IActionResult AdminHomePage()
        {
            var totalList = _iadmin.ViewListRoom();
            return View(totalList);
        }

        //call api xem thông tin phòng
        [Authorize(Roles = "Admin")]
        [Route("room/{idRoom}")]
        [HttpGet]
        public async Task<IActionResult> AdjustRoom(int idRoom)
        {
            string url = $"{_apiBaseUrl}/{idRoom}";

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {

                        var jsonString = await response.Content.ReadAsStringAsync();


                        var room = JsonConvert.DeserializeObject<AdJustRoom>(jsonString);


                        return View(room);
                    }
                    else
                    {

                        return View("Error", new { message = "Không lấy được dữ liệu từ API" });
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }



        // call api cập nhật phòng 
        [Authorize(Roles = "Admin")]
        [Route("room/{idRoom}")]
        [HttpPut]
        public async Task<IActionResult> AdjustRoom(AdJustRoom room)
        {
           

            try
            {
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm text fields
                    content.Add(new StringContent(room.RoomId.ToString()), "RoomId");
                    content.Add(new StringContent(room.RoomTypeId.ToString()), "RoomTypeId");
                    content.Add(new StringContent(room.RoomNumber ?? ""), "RoomNumber");
                    content.Add(new StringContent(room.Floor.ToString()), "Floor");
                    content.Add(new StringContent(room.PricePerNight.ToString()), "PricePerNight");
                    content.Add(new StringContent(room.Description ?? ""), "Description");

                    // Thêm list dạng nhiều row để [FromForm] bind trực tiếp
                    if (room.DeletedAmenity != null)
                    {
                        foreach (var item in room.DeletedAmenity)
                            content.Add(new StringContent(item.ToString()), "DeletedAmenity");
                    }

                    if (room.NewAmenities != null)
                    {
                        foreach (var item in room.NewAmenities)
                            content.Add(new StringContent(item.ToString()), "NewAmenities");
                    }

                    if (room.DeletedImageIds != null)
                    {
                        foreach (var item in room.DeletedImageIds)
                            content.Add(new StringContent(item.ToString()), "DeletedImageIds");
                    }

                    // Thêm file ảnh
                    if (room.NewImages != null)
                    {
                        foreach (var file in room.NewImages)
                        {
                            var fileContent = new StreamContent(file.OpenReadStream());
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                            content.Add(fileContent, "NewImages", file.FileName);
                        }
                    }

                    if (room.AvatarRoom != null)
                    {
                        // Lấy MIME type của file (nếu muốn tự động, có thể dùng room.AvatarRoom.ContentType)
                        var fileContent = new StreamContent(room.AvatarRoom.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(room.AvatarRoom.ContentType);

                        // Gửi file kèm tên file gốc
                        content.Add(fileContent, "AvatarRoom", room.AvatarRoom.FileName);
                    }

                    // gửi api và nhận bằng  HttpResponseMessage
                    HttpResponseMessage response = await client.PutAsync(_apiBaseUrl, content);

                    return response.IsSuccessStatusCode
                        ? Ok(new { success = true, message = "Cập nhật phòng thành công!" })
                        : BadRequest(new { success = false, message = "Cập nhật phòng thất bại!", detail = await response.Content.ReadAsStringAsync() });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi kết nối API!", detail = ex.Message });
            }
        }

        /*
         Note of Trung Duc  10.11.2025

            Tại sao lại không thể  chuyển thẳng object đến api 

        Reason : 
                 
               + Nếu payload chỉ các các kiểu dữ liêu thông thường như là text , number hay là các mảng thì có thể gửi dạng json thông thường 
               + Nếu payload có các kiểu dữ liệu đặc biệt gồm cả text và binary (file hay là image )  thì cần sử dụng  multipart/form-data sau đó nó sẽ tự map với các thuộc tính mà api nhận (điều kiện : tên field trùng thuộc tính)
                    
         */

        // load các thông để cần tạo phòng
        [Authorize(Roles = "Admin")]
        [Route("rooms")]
        [HttpGet]
        public IActionResult CreateNewRoom()
        {
            var data = _iadmin.LoadTypeRoomAndAmentity();

            return View(data);
        }






        // tạo phòng mới
        [Authorize(Roles = "Admin")]
        [Route("rooms")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(AdJustRoom room)
        {
            

            try
            {
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm text fields

                    content.Add(new StringContent(room.RoomTypeId.ToString()), "RoomTypeId");
                    content.Add(new StringContent(room.RoomNumber ?? ""), "RoomNumber");
                    content.Add(new StringContent(room.Floor.ToString()), "Floor");
                    content.Add(new StringContent(room.PricePerNight.ToString()), "PricePerNight");
                    content.Add(new StringContent(room.Description ?? ""), "Description");


                    if (room.NewAmenities != null)
                    {
                        foreach (var item in room.NewAmenities)
                            content.Add(new StringContent(item.ToString()), "NewAmenities");
                    }

                    // Thêm file ảnh
                    if (room.NewImages != null)
                    {
                        foreach (var file in room.NewImages)
                        {
                            var fileContent = new StreamContent(file.OpenReadStream());
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                            content.Add(fileContent, "NewImages", file.FileName);
                        }
                    }

                    if (room.AvatarRoom != null)
                    {
                        // Lấy MIME type của file (nếu muốn tự động, có thể dùng room.AvatarRoom.ContentType)
                        var fileContent = new StreamContent(room.AvatarRoom.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(room.AvatarRoom.ContentType);

                        // Gửi file kèm tên file gốc
                        content.Add(fileContent, "AvatarRoom", room.AvatarRoom.FileName);
                    }

                    // gửi api và nhận bằng  HttpResponseMessage
                    HttpResponseMessage response = await client.PostAsync(_apiBaseUrl, content);

                    return response.IsSuccessStatusCode
                        ? Ok(new { success = true, message = "Tạo phòng thành công!" })
                        : BadRequest(new { success = false, message = "Taọ phòng thất bại!", detail = await response.Content.ReadAsStringAsync() });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi kết nối API!", detail = ex.Message });
            }
        }

    }
}
