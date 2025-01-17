namespace DatabaseService.Core.Models.InputModels
{
    public class RegisterInput
    {
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string FileName { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public int? DocumentVersion { get; set; }
    }
}
