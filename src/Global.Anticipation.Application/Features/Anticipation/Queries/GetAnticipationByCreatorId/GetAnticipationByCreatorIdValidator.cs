using FluentValidation;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId
{
    public class GetAnticipationByCreatorIdValidator: AbstractValidator<GetAnticipationByCreatorIdQuery>
    {
        public GetAnticipationByCreatorIdValidator()
        {
            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("O ID do criador é obrigatório.");
        }
    }
}
