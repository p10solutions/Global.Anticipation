using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation
{
    public class UpdateAnticipationHandler(IAnticipationRepository anticipationRepository, IUnitOfWork unitOfWork, ILogger<UpdateAnticipationHandler> logger) : IRequestHandler<UpdateAnticipationInput, Result<UpdateAnticipationOutput>>
    {
        public async Task<Result<UpdateAnticipationOutput>> Handle(UpdateAnticipationInput request, CancellationToken cancellationToken)
        {
            var anticipation = await anticipationRepository.GetByIdAsync(request.Id);
            if (anticipation == null)
                return Result<UpdateAnticipationOutput>.ResultFactory.NotFound();
            
           var result = UpdateStatus(request, anticipation);

            if (result.IsFailure)
                return Result<UpdateAnticipationOutput>.ResultFactory.BusinessValidationFailure(result.Erros);

            try
            {
                anticipationRepository.Update(anticipation);
                await unitOfWork.CommitAsync();

                return Result<UpdateAnticipationOutput>
                    .ResultFactory
                    .Success(new UpdateAnticipationOutput(anticipation.Id, anticipation.Status));
            }
            catch (Exception ex)
            {
                logger
                    .LogError(ex, "Erro ao atualizar solicitação de antecipação {AnticipationId}", request.Id);

                return Result<UpdateAnticipationOutput>
                    .ResultFactory
                    .InternalError($"Erro ao atualizar solicitação de antecipação: {ex.Message}");
            }
        }

        private Result UpdateStatus(UpdateAnticipationInput request, AnticipationEntity anticipation)        
            => request.Status switch
            {
                Status.Approved => anticipation.Approve(),
                Status.Rejected => anticipation.Refuse(),
                _ => Result.ResultFactory.BusinessValidationFailure("Status não habilitado"),
            };
        
    }
}
