using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById
{
    public record GetAnticipationByIdQuery(Guid Id) : IRequest<Result<GetAnticipationByIdOutput>>
    {
    }
}
