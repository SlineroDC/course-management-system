using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        _mockConfiguration = new Mock<IConfiguration>();
        _authService = new AuthService(_mockUserManager.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new IdentityUser { Email = email, UserName = email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("supersecretkey12345678901234567890");

        var dto = new LoginDto { Email = email, Password = password };

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var email = "test@example.com";
        _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((IdentityUser)null);

        var dto = new LoginDto { Email = email, Password = "password" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(dto));
    }
}
