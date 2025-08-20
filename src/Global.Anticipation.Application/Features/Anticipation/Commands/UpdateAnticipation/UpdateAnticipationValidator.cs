using FluentValidation;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation
{
    public class UpdateAnticipationValidator: AbstractValidator<UpdateAnticipationInput>
    {
        public UpdateAnticipationValidator()
        {
            RuleFor(x=>x.Id)
                .NotEmpty().WithMessage("O ID da antecipação é obrigatório.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("O Status é obrigatório.");
        }
    }
}
