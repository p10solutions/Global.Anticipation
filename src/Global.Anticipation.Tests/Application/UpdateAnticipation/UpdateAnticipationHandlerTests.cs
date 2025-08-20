using Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation;
using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.CrossCutting.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Global.Anticipation.UnitTests.Application.UpdateAnticipation
{
    public class UpdateAnticipationHandlerTests
    {
        private readonly Mock<IAnticipationRepository> _anticipationRepositoryMock;
        private readonly Mock<ILogger<UpdateAnticipationHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UpdateAnticipationHandler _handler;

        public UpdateAnticipationHandlerTests()
        {
            _anticipationRepositoryMock = new Mock<IAnticipationRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateAnticipationHandler>>();
            _handler = new UpdateAnticipationHandler(_anticipationRepositoryMock.Object, _unitOfWork.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenApprovingAPendingAnticipation()
        {
            // Arrange
            var pendingAnticipation = new AnticipationEntity(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var anticipationId = pendingAnticipation.Id;
            var input = new UpdateAnticipationInput(anticipationId, Status.Approved);

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync(pendingAnticipation);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(anticipationId, result.Response.Id);
            Assert.Equal(Status.Approved, result.Response.Status);
            _anticipationRepositoryMock.Verify(r => r.Update(It.Is<AnticipationEntity>(a => a.Status == Status.Approved)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRejectingAPendingAnticipation()
        {
            // Arrange
            var pendingAnticipation = new AnticipationEntity(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var anticipationId = pendingAnticipation.Id;
            var input = new UpdateAnticipationInput(anticipationId, Status.Rejected);

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync(pendingAnticipation);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(anticipationId, result.Response.Id);
            Assert.Equal(Status.Rejected, result.Response.Status);
            _anticipationRepositoryMock.Verify(r => r.Update(It.Is<AnticipationEntity>(a => a.Status == Status.Rejected)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAnticipationDoesNotExist()
        {
            // Arrange
            var anticipationId = Guid.NewGuid();
            var input = new UpdateAnticipationInput(anticipationId, Status.Approved);

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync((AnticipationEntity)null);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.NotFound, result.StatusResult);
            _anticipationRepositoryMock.Verify(r => r.Update(It.IsAny<AnticipationEntity>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBusinessValidationError_WhenInputStatusIsInvalid()
        {
            // Arrange
            var existingAnticipation = new AnticipationEntity(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var anticipationId = existingAnticipation.Id;
            var input = new UpdateAnticipationInput(anticipationId, Status.Pending);

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync(existingAnticipation);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.BusinessError, result.StatusResult);
            Assert.Contains("Status não habilitado", result.Erros);
            _anticipationRepositoryMock.Verify(r => r.Update(It.IsAny<AnticipationEntity>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBusinessValidationError_WhenTryingToApproveANonPendingAnticipation()
        {
            // Arrange
            var nonPendingAnticipation = new AnticipationEntity(Guid.NewGuid(), 1500m, DateTime.UtcNow);
            nonPendingAnticipation.Approve();

            var anticipationId = nonPendingAnticipation.Id;
            var input = new UpdateAnticipationInput(anticipationId, Status.Approved);

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync(nonPendingAnticipation);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.BusinessError, result.StatusResult);
            Assert.Contains("Apenas solicitações pendentes podem ser aprovadas.", result.Erros);
            _anticipationRepositoryMock.Verify(r => r.Update(It.IsAny<AnticipationEntity>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenRepositoryUpdateThrowsException()
        {
            // Arrange
            var pendingAnticipation = new AnticipationEntity(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var anticipationId = pendingAnticipation.Id;
            var input = new UpdateAnticipationInput(anticipationId, Status.Approved);
            var exception = new Exception("Database connection error");

            _anticipationRepositoryMock.Setup(r => r.GetByIdAsync(anticipationId)).ReturnsAsync(pendingAnticipation);
            _anticipationRepositoryMock.Setup(r => r.Update(It.IsAny<AnticipationEntity>())).Throws(exception);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.ExceptionError, result.StatusResult);
            Assert.Contains($"Erro ao atualizar solicitação de antecipação: {exception.Message}", result.Erros);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Erro ao atualizar solicitação de antecipação {anticipationId}")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}