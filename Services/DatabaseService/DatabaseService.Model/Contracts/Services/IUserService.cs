using DatabaseService.Core.DataAccess.Domain;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;

namespace DatabaseService.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> Register(RegisterInput model);
        Task<LoginResult> Authenticate(LoginInput model);
        Task<ServiceResponse<string>> UserAlreadyRegistered(ExistingRegisterInput model);
        Task<List<UserResponse>> GetAllUsersAsync();
    }
}
