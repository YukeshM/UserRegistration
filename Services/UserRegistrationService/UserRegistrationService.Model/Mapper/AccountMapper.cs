using Riok.Mapperly.Abstractions;
using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Core.Mapper
{
    [Mapper]
    public partial class AccountMapper
    {
        public partial LoginResult Map(DatabaseServiceResponse source);
        public partial RegisterInputDatabase Map(RegisterInput source);
        public partial ExistingRegisterInput MapExistingRegister(RegisterInput source);
        public partial List<UserResponse> MapUser(List<UserListInput> source);


    }
}
