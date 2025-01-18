using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Core.Contracts.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse<string>> RegisterAsync(RegisterInput model);
        Task<string> LoginAsync(LoginInput model);
    }
}
