using ApiAmenity = API_BookingHotel.Modules.AmentityModules.AmentityServices;
using APIAmenityModel = API_BookingHotel.Modules.AmentityModules.AmentityServices.AmentityUpdate;
using Management_Hotel_2025.Modules.AmentityModules.AmentityServices;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.AmentityModules.AmentityControllers;

[Route("admin")]
public class ManagementAmenityController : Controller
{
    private readonly ApiAmenity.IAmenityServices _amenityService;

    public ManagementAmenityController(ApiAmenity.IAmenityServices amenityService)
    {
        _amenityService = amenityService;
    }

    private string PublicHost => $"{Request.Scheme}://{Request.Host}";

    [HttpGet("amenity")]
// Loads all amenities, maps them to the MVC view model, and displays the management list.
    public async Task<IActionResult> ViewListAmentity()
    {
        var items = await _amenityService.GetAllAmenityAsync(PublicHost);
        return View(items.Select(ToViewModel).ToList());
    }

    [HttpDelete("amenity/{id}")]
// Deletes the specified amenity and returns a success or not-found response.
    public async Task<IActionResult> DeleteAmentity(int id)
    {
        return await _amenityService.DeleteAmenityAsync(id)
            ? Ok(new { message = "XÃ³a thÃ nh cÃ´ng" })
            : NotFound("KhÃ´ng tÃ¬m tháº¥y Amenity");
    }

    [HttpPatch("amenity/{id}")]
// Changes the active status of the specified amenity.
    public async Task<IActionResult> HideAmentity(int id)
    {
        return await _amenityService.ChangeStatusAmenityAsync(id)
            ? Ok(new { status = true, message = "ThÃ nh CÃ´ng" })
            : NotFound("KhÃ´ng tÃ¬m tháº¥y Amenity");
    }

    [HttpGet("amenitys")]
// Displays the form used to create a new amenity.
    public IActionResult ViewCreateAmentity() => View();

    [HttpPost("amenity")]
// Validates the uploaded image and creates a new amenity through the amenity service.
    public async Task<IActionResult> CreateAmentity(MyAmenity request)
    {
        if (request.UpdateImage is null)
        {
            return BadRequest(new { status = false, message = "Image not found" });
        }

        var created = await _amenityService.CreateAmenityAsync(ToApiModel(request));
        return created
            ? Ok(new { status = true, message = "Create successful" })
            : BadRequest(new { message = "CÃ³ lá»—i xáº£y ra khi táº¡o tiá»‡n Ã­ch." });
    }

    [HttpGet("amenity/detail/{id}")]
// Loads one amenity and displays its data in the update form.
    public async Task<IActionResult> ViewUpdateAmentity(int id)
    {
        var item = await _amenityService.GetAmenityByIdAsync(id, PublicHost);
        return item.AmenityId is null or 0
            ? NotFound("KhÃ´ng tÃ¬m tháº¥y Amenity")
            : View(ToViewModel(item));
    }

    [HttpPut("amenity")]
// Updates the amenity data and optional image supplied by the management form.
    public async Task<IActionResult> UpdateAmentity(MyAmenity request)
    {
        var updated = await _amenityService.UpdateAmenityAsync(ToApiModel(request));
        return updated
            ? Ok(new { status = true, message = "Cáº­p nháº­t tiá»‡n Ã­ch thÃ nh cÃ´ng" })
            : BadRequest(new { message = "Cáº­p nháº­t tiá»‡n Ã­ch tháº¥t báº¡i" });
    }

    private static APIAmenityModel ToApiModel(MyAmenity item) => new()
    {
        AmenityId = item.AmenityId,
        Name = item.Name,
        Status = item.Status,
        Description = item.Description,
        UrlImage = item.UrlImage,
        UpdateImage = item.UpdateImage
    };

    private static MyAmenity ToViewModel(APIAmenityModel item) => new()
    {
        AmenityId = item.AmenityId ?? 0,
        Name = item.Name,
        Status = item.Status,
        Description = item.Description,
        UrlImage = item.UrlImage
    };
}
