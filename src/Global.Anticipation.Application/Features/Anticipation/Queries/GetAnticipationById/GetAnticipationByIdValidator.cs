using FluentValidation;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById
{
    public class GetAnticipationByIdValidator: AbstractValidator<GetAnticipationByIdQuery>
    {
        public GetAnticipationByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID é obrigatório.");
        }
    }
}
