using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation
{
    public record CreateAnticipationInput(Guid CreatorId, decimal RequestedAmount, DateTime DateRequest) : IRequest<Result<CreateAnticipationOutput>>
    {
    }
}
