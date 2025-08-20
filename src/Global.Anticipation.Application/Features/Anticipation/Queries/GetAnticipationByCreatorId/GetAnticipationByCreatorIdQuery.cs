using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId
{
    public record GetAnticipationByCreatorIdQuery(Guid CreatorId) : IRequest<Result<IEnumerable<GetAnticipationByCreatorIdOutput>>>
    {
    }
}
