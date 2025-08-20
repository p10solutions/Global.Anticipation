using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId
{
    public class GetAnticipationByCreatorIdHandler(IAnticipationRepository anticipationRepository, ILogger<GetAnticipationByCreatorIdHandler> logger): IRequestHandler<GetAnticipationByCreatorIdQuery, Result<IEnumerable<GetAnticipationByCreatorIdOutput>>>
    {
        public async Task<Result<IEnumerable<GetAnticipationByCreatorIdOutput>>> Handle(GetAnticipationByCreatorIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var anticipations = await anticipationRepository.GetByCreatorIdAsync(request.CreatorId);
                return Result<IEnumerable<GetAnticipationByCreatorIdOutput>>.ResultFactory.Success(
                    anticipations.Select(a => new GetAnticipationByCreatorIdOutput(
                        a.Id,
                        a.CreatorId,
                        a.RequestedAmount,
                        a.AppliedFee,
                        a.NetAmount,
                        a.RequestDate,
                        a.AnalysisDate,
                        a.Status                        
                    ))
                );
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Erro ao obter solicitações de antecipação para o criador {CreatorId}", request.CreatorId);
                return Result<IEnumerable<GetAnticipationByCreatorIdOutput>>.ResultFactory.InternalError($"Erro ao obter solicitações de antecipação: {ex.Message}");
            }
        }
    }
}
