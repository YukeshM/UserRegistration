using UserRegistrationService.Core.Models.ResultModels;
using UserRegistrationService.Model.Models.InputModels;

namespace UserRegistrationService.Model.Contracts.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse<string>> RegisterAsync(RegisterModel model);
        Task<LoginResultModel> LoginAsync(LoginModel model);
    }
}
