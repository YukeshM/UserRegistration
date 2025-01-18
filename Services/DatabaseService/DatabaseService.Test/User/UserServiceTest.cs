using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess;
using DatabaseService.Core.DataAccess.Domain;
using DatabaseService.Core.DataAccess.IdentityMapper;
using DatabaseService.Core.Mapper;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using DatabaseService.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Xunit;

namespace DatabaseService.Test.User
{
    public class UserServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<UserMapper> _userMapperMock;
        private readonly Mock<DbSet<Document>> _documentDbSetMock;
        private readonly Mock<UserManagementDbContext> _userManagementDbContextMock;
        private readonly IUserService _userService;

        public UserServiceTest()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);

            _userMapperMock = new Mock<UserMapper>(MockBehavior.Strict);
            _documentDbSetMock = new Mock<DbSet<Document>>();
            _userManagementDbContextMock = new Mock<UserManagementDbContext>(MockBehavior.Strict, new DbContextOptions<UserManagementDbContext>());
            _userManagementDbContextMock.Setup(x => x.Documents).Returns(_documentDbSetMock.Object);

            _userService = new UserService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _userMapperMock.Object,
                _userManagementDbContextMock.Object);
        }

        [Fact]
        public async Task Register_UserAlreadyExistsByEmail_ReturnsErrorResponse()
        {
            // Arrange
            var model = new RegisterInput { Email = "test@example.com", Username = "testuser" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _userService.Register(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this email already exists.", result.Message);
        }

        [Fact]
        public async Task Register_UserAlreadyExistsByUsername_ReturnsErrorResponse()
        {
            // Arrange
            var model = new RegisterInput { Email = "test@example.com", Username = "testuser" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _userService.Register(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this username already exists.", result.Message);
        }

        [Fact]
        public async Task Register_SuccessfulRegistration_ReturnsSuccessResponse()
        {
            // Arrange
            var model = new RegisterInput { Email = "test@example.com", Username = "testuser", Password = "Password123!" };
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = model.Username, Email = model.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _userMapperMock.Setup(x => x.MapUserDocument(model)).Returns(new Document());
            _documentDbSetMock.Setup(x => x.AddAsync(It.IsAny<Document>(), default)).Returns(new ValueTask<EntityEntry<Document>>(Task.FromResult((EntityEntry<Document>)null)));
            _userManagementDbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _userService.Register(model);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_ThrowsInvalidCredentialException()
        {
            // Arrange
            var model = new LoginInput { Email = "test@example.com", Password = "wrongpassword" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialException>(() => _userService.Authenticate(model));
        }

        [Fact]
        public async Task Authenticate_SuccessfulLogin_ReturnsLoginResult()
        {
            // Arrange
            var model = new LoginInput { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = model.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, model.Password, false, false)).ReturnsAsync(SignInResult.Success);
            _userMapperMock.Setup(x => x.Map(user)).Returns(new LoginResult());

            // Act
            var result = await _userService.Authenticate(model);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UserAlreadyRegistered_UserExistsByEmail_ReturnsErrorResponse()
        {
            // Arrange
            var model = new ExistingRegisterInput { Email = "test@example.com", Username = "testuser" };
            var users = new List<ApplicationUser> { new ApplicationUser { Email = model.Email } }.AsQueryable();
            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = await _userService.UserAlreadyRegistered(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this email already exists.", result.Message);
        }

        [Fact]
        public async Task UserAlreadyRegistered_UserExistsByUsername_ReturnsErrorResponse()
        {
            // Arrange
            var model = new ExistingRegisterInput { Email = "test@example.com", Username = "testuser" };
            var users = new List<ApplicationUser> { new ApplicationUser { UserName = model.Username } }.AsQueryable();
            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = await _userService.UserAlreadyRegistered(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this username already exists.", result.Message);
        }

        [Fact]
        public async Task UserAlreadyRegistered_UserDoesNotExist_ReturnsSuccessResponse()
        {
            // Arrange
            var model = new ExistingRegisterInput { Email = "test@example.com", Username = "testuser" };
            var users = new List<ApplicationUser>().AsQueryable();
            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = await _userService.UserAlreadyRegistered(model);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User with these details does not exist.", result.Message);
        }

        [Fact]
        public async Task Register_InvalidPassword_ReturnsErrorResponse()
        {
            // Arrange
            var model = new RegisterInput { Email = "test@example.com", Username = "testuser", Password = "short" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too short" }));

            // Act
            var result = await _userService.Register(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Registration failed", result.Message);
            Assert.Equal("Password too short", result.Message);
        }

        [Fact]
        public async Task Authenticate_UserLockedOut_ThrowsAuthenticationException()
        {
            // Arrange
            var model = new LoginInput { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = model.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, model.Password, false, false)).ReturnsAsync(SignInResult.LockedOut);

            // Act & Assert
            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.Authenticate(model));
        }
    }
}
