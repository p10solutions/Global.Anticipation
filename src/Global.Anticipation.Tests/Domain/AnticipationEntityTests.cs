using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.UnitTests.Domain
{
    public class AnticipationEntityTests
    {
        private readonly Guid _creatorId = Guid.NewGuid();
        private readonly decimal _requestedAmount = 1000m;
        private readonly DateTime _requestDate = DateTime.UtcNow;

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Act
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);

            // Assert
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(_creatorId, entity.CreatorId);
            Assert.Equal(_requestedAmount, entity.RequestedAmount);
            Assert.Equal(_requestDate, entity.RequestDate);
            Assert.Equal(Status.Pending, entity.Status);
            Assert.Null(entity.AnalysisDate);
        }

        [Fact]
        public void ApplyFee_ShouldCalculateNetAmountAndSetAppliedFeeCorrectly()
        {
            // Arrange
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);
            var feeRate = 0.03m; // 3%
            var expectedNetAmount = _requestedAmount * (1 - feeRate); // 1000 * 0.97 = 970

            // Act
            entity.ApplyFee(feeRate);

            // Assert
            Assert.Equal(feeRate, entity.AppliedFee);
            Assert.Equal(expectedNetAmount, entity.NetAmount);
        }

        [Fact]
        public void Approve_ShouldSetStatusToApproved_WhenStatusIsPending()
        {
            // Arrange
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);

            // Act
            var result = entity.Approve();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Status.Approved, entity.Status);
            Assert.NotNull(entity.AnalysisDate);
        }

        [Theory]
        [InlineData(Status.Approved)]
        [InlineData(Status.Rejected)]
        public void Approve_ShouldReturnFailure_WhenStatusIsNotPending(Status initialStatus)
        {
            // Arrange
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);

            if (initialStatus == Status.Approved) entity.Approve();
            else if (initialStatus == Status.Rejected) entity.Refuse();

            var originalAnalysisDate = entity.AnalysisDate;

            // Act
            var result = entity.Approve();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(initialStatus, entity.Status);
            Assert.Equal(originalAnalysisDate, entity.AnalysisDate); 
            Assert.Contains("Apenas solicitações pendentes podem ser aprovadas.", result.Erros);
        }

        [Fact]
        public void Refuse_ShouldSetStatusToRejected_WhenStatusIsPending()
        {
            // Arrange
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);

            // Act
            var result = entity.Refuse();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Status.Rejected, entity.Status);
            Assert.NotNull(entity.AnalysisDate);
        }

        [Theory]
        [InlineData(Status.Approved)]
        [InlineData(Status.Rejected)]
        public void Refuse_ShouldReturnFailure_WhenStatusIsNotPending(Status initialStatus)
        {
            // Arrange
            var entity = new AnticipationEntity(_creatorId, _requestedAmount, _requestDate);
            if (initialStatus == Status.Approved) entity.Approve();
            else if (initialStatus == Status.Rejected) entity.Refuse();

            var originalAnalysisDate = entity.AnalysisDate;

            // Act
            var result = entity.Refuse();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(initialStatus, entity.Status);
            Assert.Equal(originalAnalysisDate, entity.AnalysisDate);
            Assert.Contains("Apenas solicitações pendentes podem ser aprovadas.", result.Erros);
        }
    }
}