namespace DatabaseService.Core.DataAccess.Domain;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
