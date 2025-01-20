using FluentValidation;
using Microsoft.AspNetCore.Http;
using UserRegistrationService.Core.Models.InputModels;

namespace UserRegistrationService.Core.Validator
{
    public class RegisterModelValidator : AbstractValidator<RegisterInput>
    {

        private readonly string[] _allowedExtensions =
        {
            ".pdf", ".doc", ".docx",
            ".jpg", ".jpeg", ".png",
            ".xls", ".xlsx", ".csv"
        };

        public RegisterModelValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 4 and 20 characters")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Username can only contain letters and numbers");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters")
            .Matches("^[a-zA-Z ]*$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(file => file.Document)
            .NotNull().WithMessage("File is required.")
            .Must(IsValidFileType).WithMessage("Unsupported file type. Allowed types are: .pdf, .doc, .docx, .jpg, .jpeg, .png, .xls, .xlsx, .csv.")
            .Must(IsValidFileSize).WithMessage("File size must not exceed 10 MB.");
        }


        private bool IsValidFileType(IFormFile file)
        {
            if (file == null) return false;
            var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return _allowedExtensions.Contains(fileExtension);
        }

        private bool IsValidFileSize(IFormFile file)
        {
            const long maxFileSize = 10 * 1024 * 1024; // 10 MB
            return file?.Length <= maxFileSize;
        }
    }
}
