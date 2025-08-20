using FluentValidation.TestHelper;
using Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation;
using Global.Anticipation.Domain.Models.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace Global.Anticipation.UnitTests.Application.SimulateAnticipation
{
    public class SimulateAnticipationValidatorTests
    {
        private readonly SimulateAnticipationValidator _validator;
        private const decimal MinimumAmount = 25.0m;

        public SimulateAnticipationValidatorTests()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<AnticipationOptions>>();
            mockOptions.Setup(o => o.Value)
                       .Returns(new AnticipationOptions{ FeeRate = 0, MinimumRequestedAmount = MinimumAmount });

            _validator = new SimulateAnticipationValidator(mockOptions.Object);
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveNoError_WhenRequestedAmountIsGreaterThanMinimum()
        {
            // Arrange
            var validInput = new SimulateAnticipationInput(RequestedAmount: MinimumAmount + 10);

            // Act
            var result = _validator.TestValidate(validInput);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [Trait("Feature", "Anticipation - Validators")]
        [InlineData(25.0)]
        [InlineData(24.99)]
        [InlineData(0)]
        [InlineData(-100)]
        public void Should_HaveError_WhenRequestedAmountIsNotGreaterThanMinimum(decimal invalidAmount)
        {
            // Arrange
            var invalidInput = new SimulateAnticipationInput(RequestedAmount: invalidAmount);
            var expectedMessage = $"O valor solicitado deve ser maior que: R${MinimumAmount:N2}.";

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RequestedAmount)
                  .WithErrorMessage(expectedMessage);
        }
    }
}