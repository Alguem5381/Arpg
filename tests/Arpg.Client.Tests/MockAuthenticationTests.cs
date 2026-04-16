using Xunit;
using Arpg.Tests.Mocks;

namespace Arpg.Client.Tests;

public class MockAuthenticationTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var userSession = new MockUserSession();
        var authService = new MockAuthServices(userSession);
        var loginDto = new Contracts.Dto.User.LoginDto("testuser", "password123");

        // Act
        var result = await authService.LoginAsync(loginDto);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LoginAsync_WithErrorUsername_ReturnsFailure()
    {
        // Arrange
        var userSession = new MockUserSession();
        var authService = new MockAuthServices(userSession);
        var loginDto = new Contracts.Dto.User.LoginDto("error", "password123");

        // Act
        var result = await authService.LoginAsync(loginDto);

        // Assert
        Assert.True(result.IsFailed);
    }
}
