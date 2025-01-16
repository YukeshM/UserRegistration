namespace DatabaseService.Core.Models.InputModels
{
    public class RegisterInput
    {
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
