using Management_Hotel_2025.Modules.AmentityModules.AmentityServices;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.AmentityModules.AmentityControllers
{
    [Route("admin")]
    public class ManagementAmenityController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ManagementAmenityController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            // thêm tokent vào header của request   
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        [HttpGet("amenity")]
        public async Task<IActionResult> ViewListAmentity()
        {
            string url = "https://localhost:7236/api/amenity";

            using var client = CreateClientWithToken();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var amenity = JsonConvert.DeserializeObject<List<MyAmenity>>(jsonString);
                return View(amenity);
            }

            return View(new List<MyAmenity>());
        }

        [HttpDelete("amenity/{id}")]
        public async Task<IActionResult> DeleteAmentity(int id)
        {
            string url = $"https://localhost:7236/api/amenity/{id}";

            using var client = CreateClientWithToken();
            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
                return Ok(new { message = "Xóa thành công" });

            return NotFound("Không tìm thấy Amenity");
        }

        [HttpPatch("amenity/{id}")]
        public async Task<IActionResult> HideAmentity(int id)
        {
            string url = $"https://localhost:7236/api/amenity/{id}";

            using var client = CreateClientWithToken();
            var emptyContent = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PatchAsync(url, emptyContent);

            if (response.IsSuccessStatusCode)
                return Ok(new { status = true, message = "Thành Công" });

            return NotFound("Không tìm thấy Amenity");
        }

        [HttpGet("amenitys")]
        public IActionResult ViewCreateAmentity() => View();

        [HttpPost("amenity")]
        public async Task<IActionResult> CreateAmentity(MyAmenity request)
        {
            if (request.UpdateImage == null)
                return BadRequest(new { status = false, message = "Image not found" });

            string url = "https://localhost:7236/api/amenity";

            using var client = CreateClientWithToken();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.Name), "Name");
            content.Add(new StringContent(request.Status ?? ""), "Status");
            content.Add(new StringContent(request.Description ?? ""), "Description");

            var fileContent = new StreamContent(request.UpdateImage.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.UpdateImage.ContentType);
            content.Add(fileContent, "UpdateImage", request.UpdateImage.FileName);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
                return Ok(new { status = true, message = "Create successful" });

            return BadRequest(new { message = "Có lỗi xảy ra khi tạo tiện ích." });
        }

        [HttpGet("amenity/detail/{id}")]
        public async Task<IActionResult> ViewUpdateAmentity(int id)
        {
            string apiUrl = $"https://localhost:7236/api/amenity/{id}";

            using var client = CreateClientWithToken();
            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var amenity = JsonConvert.DeserializeObject<MyAmenity>(jsonString);
                return View(amenity);
            }

            return NotFound("Không tìm thấy Amenity");
        }

        [HttpPut("amenity")]
        public async Task<IActionResult> UpdateAmentity(MyAmenity request)
        {
            string url = "https://localhost:7236/api/amenity";

            using var client = CreateClientWithToken();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.AmenityId.ToString()), "AmenityId");
            content.Add(new StringContent(request.Name), "Name");
            content.Add(new StringContent(request.Description ?? ""), "Description");

            if (request.UpdateImage != null)
            {
                var fileContent = new StreamContent(request.UpdateImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.UpdateImage.ContentType);
                content.Add(fileContent, "UpdateImage", request.UpdateImage.FileName);
            }

            var response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
                return Ok(new { status = true, message = "Cập nhật tiện ích thành công" });

            return BadRequest(new { message = "Cập nhật tiện ích thất bại" });
        }
    }
}
