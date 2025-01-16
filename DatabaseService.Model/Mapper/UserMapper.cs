using DatabaseService.Core.DataAccess.IdentityMapper;
using DatabaseService.Core.Models.ResultModels;
using Riok.Mapperly.Abstractions;

namespace DatabaseService.Core.Mapper
{
    [Mapper]
    public partial class UserMapper
    {
        public partial LoginResult Map(ApplicationUser source);
    }
}
