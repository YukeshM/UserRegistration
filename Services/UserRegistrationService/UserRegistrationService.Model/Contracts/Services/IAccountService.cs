using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Core.Contracts.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse<string>> RegisterAsync(RegisterInput model);
        Task<LoginResponse> LoginAsync(LoginInput model);
        Task<List<UserResponse>> GetUsers();
    }
}
