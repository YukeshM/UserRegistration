using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text.Json;
using UserRegistrationService.Core.Contracts.Services;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ConfigurationModels;
using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Service;

public class AccountServiceTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<AccountMapper> _accountMapperMock;
    private readonly IOptions<JwtModel> _jwtConfiguration;
    private readonly IOptions<ConfigurationModel> _appConfiguration;
    private readonly IAccountService _accountService;

    public AccountServiceTest()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _accountMapperMock = new Mock<AccountMapper>();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var jwtModel = configuration.GetSection("Jwt").Get<JwtModel>();
        var configurationModel = configuration.GetSection("Configuration").Get<ConfigurationModel>();

        _jwtConfiguration = Options.Create(jwtModel);
        _appConfiguration = Options.Create(configurationModel);


        var httpClient = new HttpClient(new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Success")
        }));

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _accountService = new AccountService(_httpClientFactoryMock.Object, _accountMapperMock.Object, _jwtConfiguration, _appConfiguration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = CreateMockFormFile("test.pdf", "application/pdf")
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Success")
        }));

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await _accountService.RegisterAsync(registerInput);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Data);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenRegistrationFails()
    {
        // Set a breakpoint on the line below
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = CreateMockFormFile("test.pdf", "application/pdf")
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Error")
        }));

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await _accountService.RegisterAsync(registerInput);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenDocumentIsNull()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _accountService.RegisterAsync(registerInput));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenDocumentIsEmpty()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = CreateMockFormFile("test.pdf", "application/pdf", 0)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _accountService.RegisterAsync(registerInput));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenFileNameIsInvalid()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = CreateMockFormFile("test<>.pdf", "application/pdf")
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _accountService.RegisterAsync(registerInput));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenFolderPathIsInvalid()
    {
        // Arrange
        var registerInput = new RegisterInput
        {
            Username = "testuser",
            LastName = "Test",
            Email = "testuser@example.com",
            Password = "Password123!",
            RegistrationDate = DateTime.UtcNow,
            Document = CreateMockFormFile("test.pdf", "application/pdf")
        };

        var invalidFolderPath = new string(Path.GetInvalidPathChars());

        var appConfiguration = new ConfigurationModel { FolderPath = invalidFolderPath, DBUrl = _appConfiguration.Value.DBUrl };

        var localAppConfiguration = Options.Create(appConfiguration);

        var localAccountService = new AccountService(_httpClientFactoryMock.Object, _accountMapperMock.Object, _jwtConfiguration, localAppConfiguration);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => localAccountService.RegisterAsync(registerInput));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginInput = new LoginInput
        {
            Email = "testuser",
            Password = "Password123!"
        };

        var databaseResponse = new DatabaseServiceResponse
        {
            UserName = "testuser@123",
            Id = new Guid().ToString(),
            Email = "testuser"
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(databaseResponse))
        }));

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);


        // Act
        var token = await _accountService.LoginAsync(loginInput);

        // Assert
        Assert.NotNull(token);
        Assert.IsType<LoginResponse>(token);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenLoginFails()
    {
        // Arrange
        var loginInput = new LoginInput
        {
            Email = "testuser",
            Password = "Password123!"
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Content = new StringContent("Unauthorized")
        }));

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _accountService.LoginAsync(loginInput));
    }


    private IFormFile CreateMockFormFile(string fileName, string contentType, long length = 1024)
    {
        var fileMock = new Mock<IFormFile>();
        var content = new string('a', (int)length);
        var fileNameWithExtension = fileName;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileNameWithExtension);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(_ => _.ContentType).Returns(contentType);
        return fileMock.Object;
    }
}

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _responseMessage;

    public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        return Task.FromResult(_responseMessage);
    }
}
