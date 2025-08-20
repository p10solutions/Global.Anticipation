using Global.Anticipation.Infra.CrossCutting.Results;

namespace Global.Anticipation.Domain.Entities;

public class AnticipationEntity
{
    public Guid Id { get; private set; }
    public Guid CreatorId { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public decimal AppliedFee { get; private set; }
    public decimal NetAmount { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime? AnalysisDate { get; private set; }
    public Status Status { get; private set; }

    private AnticipationEntity() { }

    public AnticipationEntity(Guid creatorId, decimal requestedAmount, DateTime requestDate)
    {
        CreatorId = creatorId;
        RequestedAmount = requestedAmount;
        RequestDate = requestDate;
        Id = Guid.NewGuid();
        Status = Status.Pending;
    }

    public void ApplyFee(decimal feeRate)
    {
        NetAmount = RequestedAmount - (RequestedAmount * feeRate);
        AppliedFee = feeRate;
    }

    public Result Approve()
    {
        if (Status != Status.Pending)
            return Result.ResultFactory.BusinessValidationFailure("Apenas solicitações pendentes podem ser aprovadas.");

        Status = Status.Approved;
        AnalysisDate = DateTime.UtcNow;

        return Result.ResultFactory.Success();
    }

    public Result Refuse()
    {
        if (Status != Status.Pending)
            return Result.ResultFactory.BusinessValidationFailure("Apenas solicitações pendentes podem ser aprovadas.");

        Status = Status.Rejected;
        AnalysisDate = DateTime.UtcNow;

        return Result.ResultFactory.Success();
    }
}