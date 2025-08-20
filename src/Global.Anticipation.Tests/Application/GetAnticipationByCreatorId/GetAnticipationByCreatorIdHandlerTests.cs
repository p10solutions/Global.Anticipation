using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId;
using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.CrossCutting.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Global.Anticipation.UnitTests.Application.GetAnticipationByCreatorId
{
    public class GetAnticipationByCreatorIdHandlerTests
    {
        private readonly Mock<IAnticipationRepository> _anticipationRepositoryMock;
        private readonly Mock<ILogger<GetAnticipationByCreatorIdHandler>> _loggerMock;
        private readonly GetAnticipationByCreatorIdHandler _handler;

        public GetAnticipationByCreatorIdHandlerTests()
        {
            _anticipationRepositoryMock = new Mock<IAnticipationRepository>();
            _loggerMock = new Mock<ILogger<GetAnticipationByCreatorIdHandler>>();
            _handler = new GetAnticipationByCreatorIdHandler(_anticipationRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithMappedData_WhenAnticipationsAreFound()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var query = new GetAnticipationByCreatorIdQuery(creatorId);

            var approvedAnticipation = new AnticipationEntity(creatorId, 1000m, DateTime.UtcNow.AddDays(-5));
            approvedAnticipation.Approve();

            var pendingAnticipation = new AnticipationEntity(creatorId, 2000m, DateTime.UtcNow);

            var anticipationsFromRepo = new List<AnticipationEntity> { approvedAnticipation, pendingAnticipation };

            _anticipationRepositoryMock
                .Setup(r => r.GetByCreatorIdAsync(creatorId))
                .ReturnsAsync(anticipationsFromRepo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(2, result.Response.Count());

            var firstResult = result.Response.First();
            var firstEntity = anticipationsFromRepo.First();
            Assert.Equal(firstEntity.Id, firstResult.Id);
            Assert.Equal(firstEntity.RequestedAmount, firstResult.RequestedAmount);
            Assert.Equal(Status.Approved, firstResult.Status);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenNoAnticipationsAreFound()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var query = new GetAnticipationByCreatorIdQuery(creatorId);
            var emptyList = new List<AnticipationEntity>();

            _anticipationRepositoryMock
                .Setup(r => r.GetByCreatorIdAsync(creatorId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Empty(result.Response);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenRepositoryThrowsException()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var query = new GetAnticipationByCreatorIdQuery(creatorId);
            var exception = new Exception("Database error");

            _anticipationRepositoryMock
                .Setup(r => r.GetByCreatorIdAsync(creatorId))
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
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Erro ao obter solicitações de antecipação para o criador {creatorId}")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}