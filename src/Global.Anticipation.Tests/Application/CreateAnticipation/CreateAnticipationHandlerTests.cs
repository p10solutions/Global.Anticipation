using Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation;
using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Domain.Models.Options;
using Global.Anticipation.Infra.CrossCutting.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;


namespace Global.Anticipation.UnitTests.Application.CreateAnticipation
{
    public class CreateAnticipationHandlerTests
    {
        private readonly Mock<IAnticipationRepository> _anticipationRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateAnticipationHandler>> _loggerMock;
        private readonly Mock<IOptions<AnticipationOptions>> _feeOptionsMock;
        private readonly CreateAnticipationHandler _handler;

        public CreateAnticipationHandlerTests()
        {
            // Arrange
            _anticipationRepositoryMock = new Mock<IAnticipationRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateAnticipationHandler>>();
            _feeOptionsMock = new Mock<IOptions<AnticipationOptions>>();

            _feeOptionsMock.Setup(o => o.Value).Returns(new AnticipationOptions { FeeRate = 0.03m, MinimumRequestedAmount = 100 });

            _handler = new CreateAnticipationHandler(
                _anticipationRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _feeOptionsMock.Object
            );
        }

        [Fact]
        [Trait("Feature", "Anticipation - Commands")]
        public async Task Handle_ShouldReturnSuccess_WhenCreatorHasNoPendingRequest()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var requestedAmount = 1000m;
            var feeRate = _feeOptionsMock.Object.Value.FeeRate;
            var expectedNetAmount = requestedAmount * (1 - feeRate);

            var input = new CreateAnticipationInput(creatorId, requestedAmount, DateTime.UtcNow);

            _anticipationRepositoryMock
                .Setup(r => r.HasPendingRequestForCreatorAsync(creatorId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response); 
            Assert.Equal(expectedNetAmount, result.Response.NetAmount);
            Assert.Equal(Status.Pending, result.Response.Status); 

            _anticipationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AnticipationEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        [Trait("Feature", "Anticipation - Commands")]
        public async Task Handle_ShouldReturnBusinessValidationFailure_WhenCreatorHasPendingRequest()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var input = new CreateAnticipationInput(creatorId, 1000m, DateTime.UtcNow);
            var expectedErrorMessage = "O criador já possui uma solicitação pendente.";

            _anticipationRepositoryMock
                .Setup(r => r.HasPendingRequestForCreatorAsync(creatorId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.BusinessError, result.StatusResult);
            Assert.Contains(expectedErrorMessage, result.Erros);

            _anticipationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AnticipationEntity>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        [Trait("Feature", "Anticipation - Commands")]
        public async Task Handle_ShouldReturnInternalError_WhenCommitThrowsException()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var input = new CreateAnticipationInput(creatorId, 1000m, DateTime.UtcNow);
            var exceptionMessage = "Database commit failed";
            var dbException = new InvalidOperationException(exceptionMessage);

            _anticipationRepositoryMock
                .Setup(r => r.HasPendingRequestForCreatorAsync(creatorId))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .ThrowsAsync(dbException);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.ExceptionError, result.StatusResult);
            Assert.Contains($"Erro ao criar solicitação de antecipação: {exceptionMessage}", result.Erros);

            _anticipationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AnticipationEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Erro ao criar solicitação de antecipação para o criador {creatorId}")),
                    dbException,
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}