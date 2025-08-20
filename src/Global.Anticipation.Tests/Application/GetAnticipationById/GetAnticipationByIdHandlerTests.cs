using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById;
using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.CrossCutting.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Global.Anticipation.UnitTests.Application.GetAnticipationById
{
    public class GetAnticipationByIdHandlerTests
    {
        private readonly Mock<IAnticipationRepository> _anticipationRepositoryMock;
        private readonly Mock<ILogger<GetAnticipationByIdHandler>> _loggerMock;
        private readonly GetAnticipationByIdHandler _handler;

        public GetAnticipationByIdHandlerTests()
        {
            _anticipationRepositoryMock = new Mock<IAnticipationRepository>();
            _loggerMock = new Mock<ILogger<GetAnticipationByIdHandler>>();
            _handler = new GetAnticipationByIdHandler(_anticipationRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAnticipationIsFound()
        {
            // Arrange
            var anticipationEntity = new AnticipationEntity(Guid.NewGuid(), 1500m, DateTime.UtcNow);
            var query = new GetAnticipationByIdQuery(anticipationEntity.Id);

            _anticipationRepositoryMock
                .Setup(r => r.GetByIdAsync(anticipationEntity.Id))
                .ReturnsAsync(anticipationEntity);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(anticipationEntity.Id, result.Response.Id);
            Assert.Equal(anticipationEntity.CreatorId, result.Response.CreatorId);
            Assert.Equal(anticipationEntity.RequestedAmount, result.Response.RequestedAmount);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAnticipationIsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var query = new GetAnticipationByIdQuery(nonExistentId);

            _anticipationRepositoryMock
                .Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((AnticipationEntity)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.NotFound, result.StatusResult);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenRepositoryThrowsException()
        {
            // Arrange
            var query = new GetAnticipationByIdQuery(Guid.NewGuid());
            var exception = new Exception("Falha na conexão com o banco de dados");

            _anticipationRepositoryMock
                .Setup(r => r.GetByIdAsync(query.Id))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(EStatusResult.ExceptionError, result.StatusResult);
            Assert.Contains($"Erro ao obter solicitações de antecipação: {exception.Message}", result.Erros);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Erro ao obter solicitações de antecipação para o Id {query.Id}")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}