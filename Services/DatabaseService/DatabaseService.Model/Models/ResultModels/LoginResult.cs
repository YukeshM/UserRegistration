using System.Text.Json.Serialization;

namespace DatabaseService.Core.Models.ResultModels
{
    public class LoginResult
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
