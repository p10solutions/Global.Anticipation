using FluentValidation.TestHelper;
using Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation;
using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.UnitTests.Application.UpdateAnticipation
{
    public class UpdateAnticipationValidatorTests
    {
        private readonly UpdateAnticipationValidator _validator;

        public UpdateAnticipationValidatorTests()
        {
            _validator = new UpdateAnticipationValidator();
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveNoError_WhenInputIsValid()
        {
            // Arrange
            var validInput = new UpdateAnticipationInput(
                Id: Guid.NewGuid(),
                Status: Status.Approved
            );

            // Act
            var result = _validator.TestValidate(validInput);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveError_WhenIdIsEmpty()
        {
            // Arrange
            var invalidInput = new UpdateAnticipationInput(
                Id: Guid.Empty,
                Status: Status.Rejected
            );

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("O ID da antecipação é obrigatório.");
        }

        [Fact]
        [Trait("Feature", "Anticipation - Validators")]
        public void Should_HaveError_WhenStatusIsEmpty()
        {
            // Arrange
            var invalidInput = new UpdateAnticipationInput(
                Id: Guid.NewGuid(),
                Status: default
            );

            // Act
            var result = _validator.TestValidate(invalidInput);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Status)
                  .WithErrorMessage("O Status é obrigatório.");
        }
    }
}