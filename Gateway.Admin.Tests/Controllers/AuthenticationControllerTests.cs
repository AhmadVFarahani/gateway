using Gateway.Admin.Api.Controllers;
using Gateway.Application.Authentication.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Gateway.Admin.Tests.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task Authenticate_Should_Return_BadRequest_When_ApiKey_Is_Missing()
    {
        // Arrange
        var mockApiKeyRepo = new Mock<IApiKeyRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockJwtService = new Mock<IJwtService>();

        var controller = new AuthenticationController(
            mockApiKeyRepo.Object,
            mockUserRepo.Object,
            mockJwtService.Object
        );

        var request = new AuthenticateRequest
        {
            ApiKey = "" //  Empty API Key 
        };

        // Act
        var result = await controller.Authenticate(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("API Key is required.", badRequestResult.Value);
    }
    [Fact]
    public async Task Authenticate_Should_Return_Unauthorized_When_ApiKey_Is_Invalid()
    {
        // Arrange
        var mockApiKeyRepo = new Mock<IApiKeyRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockJwtService = new Mock<IJwtService>();

        mockApiKeyRepo
            .Setup(repo => repo.GetByKeyAsync("invalid-key"))
            .ReturnsAsync((ApiKey?)null); //ApiKey Not found

        var controller = new AuthenticationController(
            mockApiKeyRepo.Object,
            mockUserRepo.Object,
            mockJwtService.Object
        );

        var request = new AuthenticateRequest
        {
            ApiKey = "invalid-key"
        };

        // Act
        var result = await controller.Authenticate(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid or expired API Key.", unauthorizedResult.Value);
    }

[Fact]
    public async Task Authenticate_Should_Return_Unauthorized_When_User_Is_Inactive()
    {
        // Arrange
        var mockApiKeyRepo = new Mock<IApiKeyRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockJwtService = new Mock<IJwtService>();

        var fakeApiKey = new ApiKey
        {
            Key = "valid-key",
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddMinutes(30),
            UserId = 123
        };

        var inactiveUser = new User
        {
            Id = 123,
            IsActive = false
        };

        mockApiKeyRepo
            .Setup(repo => repo.GetByKeyAsync("valid-key"))
            .ReturnsAsync(fakeApiKey);

        mockUserRepo
            .Setup(repo => repo.GetByIdAsync(123))
            .ReturnsAsync(inactiveUser);

        var controller = new AuthenticationController(
            mockApiKeyRepo.Object,
            mockUserRepo.Object,
            mockJwtService.Object
        );

        var request = new AuthenticateRequest
        {
            ApiKey = "valid-key"
        };

        // Act
        var result = await controller.Authenticate(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("User is inactive.", unauthorizedResult.Value);
    }
    [Fact]
    public async Task Authenticate_Should_Return_Ok_With_Token_When_Request_Is_Valid()
    {
        // Arrange
        var mockApiKeyRepo = new Mock<IApiKeyRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockJwtService = new Mock<IJwtService>();

        var apiKey = new ApiKey
        {
            Key = "valid-key",
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddMinutes(60),
            UserId = 1
        };

        var user = new User
        {
            Id = 1,
            IsActive = true
        };

        mockApiKeyRepo
            .Setup(r => r.GetByKeyAsync("valid-key"))
            .ReturnsAsync(apiKey);

        mockUserRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        mockJwtService
            .Setup(s => s.GenerateToken(user, apiKey))
            .Returns("mocked-jwt-token");

        // برای IConfiguration باید یک مقدار شبیه‌سازی‌شده ست کنیم
        var controller = new AuthenticationController(
            mockApiKeyRepo.Object,
            mockUserRepo.Object,
            mockJwtService.Object
        );

        // برای Expiration از IConfiguration می‌گیره
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
            {"JwtSettings:ExpiresInMinutes", "30"}
            })
            .Build();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                    .AddSingleton<IConfiguration>(configuration)
                    .BuildServiceProvider()
            }
        };

        var request = new AuthenticateRequest
        {
            ApiKey = "valid-key"
        };

        // Act
        var result = await controller.Authenticate(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthenticateResponse>(okResult.Value);

        Assert.Equal("mocked-jwt-token", response.Token);
        Assert.True(response.Expiration > DateTime.UtcNow);
    }

}
