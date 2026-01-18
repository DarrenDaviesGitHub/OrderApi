using Ardent.OrderApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.OrderApi.UnitTest.Controllers;

public class HealthControllerTests
{
    private HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public async Task Get_ShouldReturnOrderAPIOnlineResponse()
    {
        // Arrange
        const string expectedResponse = "Order API is online";

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResponse);
    }
}
