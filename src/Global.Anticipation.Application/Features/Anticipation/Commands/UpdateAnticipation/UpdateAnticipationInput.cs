using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation
{
    public record UpdateAnticipationInput(Guid Id, Status Status) : IRequest<Result<UpdateAnticipationOutput>>
    {
    }
}
