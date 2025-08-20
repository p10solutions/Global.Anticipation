using Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Domain.Models.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace Global.Anticipation.UnitTests.Application.SimulateAnticipation
{
    public class SimulateAnticipationHandlerTests
    {
        private readonly Mock<IOptions<AnticipationOptions>> _mockOptions;
        private readonly SimulateAnticipationHandler _handler;
        private const decimal FeeRate = 0.05m;

        public SimulateAnticipationHandlerTests()
        {
            // Arrange
            _mockOptions = new Mock<IOptions<AnticipationOptions>>();
            _mockOptions.Setup(o => o.Value)
                        .Returns(new AnticipationOptions { FeeRate = FeeRate, MinimumRequestedAmount = 100 });

            _handler = new SimulateAnticipationHandler(_mockOptions.Object);
        }

        [Theory]
        [Trait("Feature", "Anticipation - Commands")]
        [InlineData(1000.0, 950.0)]
        [InlineData(150.50, 142.975)]
        [InlineData(0.0, 0.0)]
        public async Task Handle_ShouldReturnSuccess_WithCorrectlyCalculatedNetAmount(decimal requestedAmount, decimal expectedNetAmount)
        {
            // Arrange
            var input = new SimulateAnticipationInput(requestedAmount);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(expectedNetAmount, result.Response.NetAmount);
        }
    }
}