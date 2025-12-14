using FluentAssertions;
using Moq;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;
using MesClient.Infrastructure.Services;
using Serilog;
using Xunit;

namespace MesClient.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IApiClient> _apiClientMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _apiClientMock = new Mock<IApiClient>();
        _loggerMock = new Mock<ILogger>();

        _authService = new AuthService(_apiClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void InitialState_ShouldNotBeAuthenticated()
    {
        // Assert
        _authService.IsAuthenticated.Should().BeFalse();
        _authService.CurrentUser.Should().BeNull();
    }

    [Fact]
    public async Task LogoutAsync_ShouldClearUserAndToken()
    {
        // Arrange
        _apiClientMock
            .Setup(x => x.PostAsync<object, object>(It.IsAny<string>(), It.IsAny<object>(), default))
            .ReturnsAsync(new object());

        // Act
        await _authService.LogoutAsync();

        // Assert
        _authService.IsAuthenticated.Should().BeFalse();
        _authService.CurrentUser.Should().BeNull();
    }

    [Fact]
    public async Task LogoutAsync_ShouldRaiseAuthStateChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _authService.AuthStateChanged += (s, e) =>
        {
            eventRaised = true;
            e.IsAuthenticated.Should().BeFalse();
            e.User.Should().BeNull();
        };

        // Act
        await _authService.LogoutAsync();

        // Assert
        eventRaised.Should().BeTrue();
    }
}
