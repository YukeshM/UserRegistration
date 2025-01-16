using FluentValidation;
using UserRegistrationService.Model.Models.InputModels;

namespace UserRegistrationService.Validator
{
    public class LoginModelValidator : AbstractValidator<LoginInput>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }

}
