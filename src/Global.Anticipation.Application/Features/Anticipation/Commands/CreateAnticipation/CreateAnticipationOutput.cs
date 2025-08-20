using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation
{
    public record CreateAnticipationOutput
    {
        public Guid Id { get; init; }
        public decimal NetAmount { get; init; }
        public Status Status { get; init; }
    }
}
