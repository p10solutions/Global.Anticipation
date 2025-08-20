using FluentValidation.TestHelper;
using Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation;
using Global.Anticipation.Domain.Models.Options; 
using Microsoft.Extensions.Options;
using Moq;

namespace Global.Anticipation.Application.Tests.Features.Anticipation.Validators
{
    public class CreateAnticipationValidatorTests
    {
        private readonly CreateAnticipationValidator _validator;
        private readonly Mock<IOptions<AnticipationOptions>> _mockOptions;
        private const decimal MinimumAmount = 50.0m;

        public CreateAnticipationValidatorTests()
        {
            // Arrange
            _mockOptions = new Mock<IOptions<AnticipationOptions>>();
            _mockOptions.Setup(o => o.Value)
                        .Returns(new AnticipationOptions { FeeRate = 0.00m, MinimumRequestedAmount = MinimumAmount });

            _validator = new CreateAnticipationValidator(_mockOptions.Object);
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveNoError_WhenInputIsValid()
        {
            // Arrange
            var validInput = new CreateAnticipationInput(
                CreatorId: Guid.NewGuid(),
                DateRequest: DateTime.UtcNow,
                RequestedAmount: MinimumAmount + 100
            );

            // Act
            var result = _validator.TestValidate(validInput);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveError_WhenCreatorIdIsEmpty()
        {
            // Arrange
            var invalidInput = new CreateAnticipationInput(
                CreatorId: Guid.Empty,
                DateRequest: DateTime.UtcNow,
                RequestedAmount: 100m
            );

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CreatorId)
                  .WithErrorMessage("O ID do criador é obrigatório.");
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveError_WhenDateRequestIsEmpty()
        {
            // Arrange
            var invalidInput = new CreateAnticipationInput(
                CreatorId: Guid.NewGuid(),
                DateRequest: default,
                RequestedAmount: 100m
            );

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DateRequest)
                  .WithErrorMessage("A data da solicitação é obrigatória.");
        }

        [Theory]
        [Trait("Feature", "Anticipation - Validators")]
        [InlineData(50.0)]
        [InlineData(49.99)]
        [InlineData(0)]
        [InlineData(-100)]
        public void Should_HaveError_WhenRequestedAmountIsNotGreaterThanMinimum(decimal invalidAmount)
        {
            // Arrange
            var invalidInput = new CreateAnticipationInput(
                CreatorId: Guid.NewGuid(),
                DateRequest: DateTime.UtcNow,
                RequestedAmount: invalidAmount
            );
            var expectedMessage = $"O valor solicitado deve ser maior que: R${MinimumAmount:N2}.";

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RequestedAmount)
                  .WithErrorMessage(expectedMessage);
        }
    }
}