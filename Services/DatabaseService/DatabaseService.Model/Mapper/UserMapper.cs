using DatabaseService.Core.DataAccess.Domain;
using DatabaseService.Core.DataAccess.IdentityModel;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using Riok.Mapperly.Abstractions;

namespace DatabaseService.Core.Mapper
{
    [Mapper]
    public partial class UserMapper
    {
        public partial LoginResult Map(ApplicationUser source);
        public partial Document MapUserDocument(RegisterInput source);
    }
}
