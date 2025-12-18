using System.Text.Json;
using System.Text;

namespace Management_Hotel_2025.Modules.CallAPI
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

    }
}
