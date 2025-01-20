using System.Text.Json.Serialization;

namespace UserRegistrationService.Core.Models.ResponseModels
{
    public class DatabaseServiceResponse
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

}
