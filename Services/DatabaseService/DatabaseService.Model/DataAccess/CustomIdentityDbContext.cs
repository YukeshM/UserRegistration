using DatabaseService.Core.DataAccess.IdentityModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Core.DataAccess;

public partial class CustomIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
       ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
{

    public CustomIdentityDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(e => e.UserName).HasColumnName("FirstName");
            b.ToTable("User");
        });

        modelBuilder.Entity<ApplicationRole>(b =>
        {
            b.ToTable("Role");
        });

        modelBuilder.Entity<ApplicationRoleClaim>(b =>
        {
            b.ToTable("RoleClaim");
        });

        modelBuilder.Entity<ApplicationUserClaim>(b =>
        {
            b.ToTable("UserClaim");
        });

        modelBuilder.Entity<ApplicationUserLogin>(b =>
        {
            b.ToTable("UserLogin");
        });

        modelBuilder.Entity<ApplicationUserRole>(b =>
        {
            b.ToTable("UserRole");
        });

        modelBuilder.Entity<ApplicationUserToken>(b =>
        {
            b.ToTable("UserToken");
        });
    }
}
