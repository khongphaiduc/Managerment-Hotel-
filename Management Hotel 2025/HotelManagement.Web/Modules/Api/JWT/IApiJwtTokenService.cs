namespace API_BookingHotel.Modules.JWT;

public interface IApiJwtTokenService
{
    string Generate(string name, string role);
}
