using FluentValidation;
using UserRegistrationService.Core.Models.InputModels;

namespace UserRegistrationService.Core.Validator
{
    public class LoginModelValidator : AbstractValidator<LoginInput>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
