using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId
{
    public record GetAnticipationByCreatorIdOutput(
        Guid Id,
        Guid CreatorId,
        decimal RequestedAmount,
        decimal AppliedFee,
        decimal NetAmount,
        DateTime RequestDate,
        DateTime? AnalysisDate,
        Status Status
    );
}
