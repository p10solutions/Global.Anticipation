using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation
{
    public record UpdateAnticipationOutput(Guid Id, Status Status)
    {
    }
}
