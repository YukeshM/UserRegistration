using Riok.Mapperly.Abstractions;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Core.Mapper
{
    [Mapper]
    public partial class AccountMapper
    {
        public partial LoginResult Map(DatabaseServiceResponse source);
    }
}
