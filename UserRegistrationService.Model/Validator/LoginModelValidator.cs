using FluentValidation;
using UserRegistrationService.Model.Models.InputModels;

namespace UserRegistrationService.Core.Validator
{
    public class LoginModelValidator : AbstractValidator<LoginInput>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        }
    }
}
