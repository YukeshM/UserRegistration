namespace DatabaseService.Core.Models.InputModels;

public class UserInput
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public string Email { get; set; } = null!;
}
