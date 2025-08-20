using FluentValidation;
using Global.Anticipation.Domain.Models.Options;
using Microsoft.Extensions.Options;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation
{
    public class SimulateAnticipationValidator: AbstractValidator<SimulateAnticipationInput>
    {
        public SimulateAnticipationValidator(IOptions<AnticipationOptions> options)
        {
            RuleFor(x => x.RequestedAmount)
                        .GreaterThan(options.Value.MinimumRequestedAmount)
                        .WithMessage(x => $"O valor solicitado deve ser maior que: R${options.Value.MinimumRequestedAmount:N2}.");
        }
    }
}
