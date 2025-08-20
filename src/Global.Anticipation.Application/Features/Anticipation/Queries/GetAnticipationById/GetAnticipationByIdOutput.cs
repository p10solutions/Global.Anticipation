using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById
{
    public record GetAnticipationByIdOutput(
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
