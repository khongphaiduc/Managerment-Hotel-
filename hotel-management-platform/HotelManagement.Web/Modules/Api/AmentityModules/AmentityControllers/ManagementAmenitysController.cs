using API_BookingHotel.Modules.AmentityModules.AmentityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace API_BookingHotel.Modules.AmentityModules.AmentityControllers
{
    [Authorize(Roles = "Admin")]
    [Route("api")]
    [ApiController]
    public class ManagementAmenitysController : ControllerBase
    {
        private readonly IAmenityServices _imentity;

        public ManagementAmenitysController(IAmenityServices amenity)
        {
            _imentity = amenity;
        }



        [Route("amenity")]// ok
        [HttpPost]

// Creates an amenity from the request data and returns the operation status.
        public async Task<IActionResult> CreateAmentity(AmentityUpdate amentity)
        {

            var result = await _imentity.CreateAmenityAsync(amentity);

            if (!result)
            {
                return BadRequest(new { message = "Tạo mới tiện ích thất bại" });
            }
            else
            {
                return Ok(new { message = "Tạo mới tiện ích thành công" });
            }

        }




        [HttpGet("amenity/{id}")]// ok 
// Gets one amenity by identifier, including its public image URL when available.
        public async Task<IActionResult> GetAmentity(int id)
        {
            string apihost = $"{Request.Scheme}://{Request.Host}";
            var result = await _imentity.GetAmenityByIdAsync(id, apihost);

            if (result == null)
            {
                return NotFound(new { message = "Lấy amentity thất bại hoặc amentity không tồn tại" });
            }
            else
            {
                return Ok(result);
            }

        }


        [HttpGet("amenity")]// ok 
// Gets the complete amenity collection for the admin client.
        public async Task<IActionResult> GetAmentityAll()
        {
            string apihost = $"{Request.Scheme}://{Request.Host}";
            var result = await _imentity.GetAllAmenityAsync(apihost);

            if (result == null)
            {
                return BadRequest(new { message = "Lấy danh sách amentity thất bại hoặc amentity không tồn tại" });
            }
            else
            {
                return Ok(result);
            }

        }


        [HttpPut("amenity")] // ok 
// Updates an existing amenity after validating its identifier.
        public async Task<IActionResult> UpdateAmentity(AmentityUpdate request)
        {
            if (request.AmenityId == null || request.AmenityId <= 0)
            {
                return NotFound(new { message = "Không tìm thấy Amentity cần update" });
            }

            var result = await _imentity.UpdateAmenityAsync(request);

            if (!result)
            {
                return BadRequest(new { message = "Cập nhật amentity thất bại" });
            }
            else
            {
                return Ok(new { message = "Cập nhật amentity thành công" });
            }

        }


        [HttpPatch("amenity/{id}")]
// Toggles the status of an amenity identified by route parameter.
        public async Task<IActionResult> UpdateStatusAmentity(int id)
        {

            var result = await _imentity.ChangeStatusAmenityAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "Cập nhật trạng thái amentity thất bại" });
            }
            else
            {
                return Ok(new { message = "Cập nhật trạng thái amentity thành công" });
            }

        }


        [HttpDelete("amenity/{id}")]  // ok 
// Deletes an amenity identified by route parameter.
        public async Task<IActionResult> DeleteAmentity(int id)
        {

            var result = await _imentity.DeleteAmenityAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "Xóa amentity thất bại" });
            }
            else
            {
                return Ok(new { message = "Xóa amentity thành công" });
            }

        }



    }
}
