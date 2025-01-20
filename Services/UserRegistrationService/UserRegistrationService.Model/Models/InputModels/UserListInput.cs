using System.Text.Json.Serialization;

namespace UserRegistrationService.Core.Models.InputModels
{
    public class UserListInput
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
