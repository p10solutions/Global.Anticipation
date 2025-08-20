using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Domain.Models.Options;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation
{
    public class CreateAnticipationHandler
    (
        IAnticipationRepository anticipationRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAnticipationHandler> logger,
        IOptions<AnticipationOptions> feeOptions
    )
    : IRequestHandler<CreateAnticipationInput, Result<CreateAnticipationOutput>>
    {
        public async Task<Result<CreateAnticipationOutput>> Handle(CreateAnticipationInput request, CancellationToken cancellationToken)
        {
            if (await anticipationRepository.HasPendingRequestForCreatorAsync(request.CreatorId))
                return Result<CreateAnticipationOutput>.ResultFactory.BusinessValidationFailure("O criador já possui uma solicitação pendente.");

            var anticipation = new AnticipationEntity(request.CreatorId, request.RequestedAmount, request.DateRequest);

            anticipation.ApplyFee(feeOptions.Value.FeeRate);

            try
            {
                await anticipationRepository.AddAsync(anticipation);
                await unitOfWork.CommitAsync();

                return Result<CreateAnticipationOutput>
                    .ResultFactory
                    .Success(
                        new CreateAnticipationOutput
                        {
                            Id = anticipation.Id,
                            NetAmount = anticipation.NetAmount,
                            Status = anticipation.Status
                        });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar solicitação de antecipação para o criador {CreatorId}", request.CreatorId);

                return Result<CreateAnticipationOutput>
                    .ResultFactory
                    .InternalError($"Erro ao criar solicitação de antecipação: {ex.Message}");
            }
        }
    }
}
