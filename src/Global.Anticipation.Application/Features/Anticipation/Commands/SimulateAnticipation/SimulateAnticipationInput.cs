using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation
{
    public record SimulateAnticipationInput(decimal RequestedAmount) : IRequest<Result<SimulateAnticipationOutput>>
    {
    }
}
