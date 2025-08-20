using FluentValidation;
using Global.Anticipation.Domain.Models.Options;
using Microsoft.Extensions.Options;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation
{
    public class CreateAnticipationValidator: AbstractValidator<CreateAnticipationInput>
    {
        public CreateAnticipationValidator(IOptions<AnticipationOptions> options)
        {
            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("O ID do criador é obrigatório.");
            
            RuleFor(x => x.DateRequest) 
                .NotEmpty().WithMessage("A data da solicitação é obrigatória.");

            RuleFor(x => x.RequestedAmount)
                .GreaterThan(options.Value.MinimumRequestedAmount)
                .WithMessage(x => $"O valor solicitado deve ser maior que: R${options.Value.MinimumRequestedAmount:N2}.");
        }
    }
}
