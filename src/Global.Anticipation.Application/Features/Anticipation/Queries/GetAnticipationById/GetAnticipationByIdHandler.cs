using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById
{
    public class GetAnticipationByIdHandler(IAnticipationRepository anticipationRepository, ILogger<GetAnticipationByIdHandler> logger) : IRequestHandler<GetAnticipationByIdQuery, Result<GetAnticipationByIdOutput>>
    {
        public async Task<Result<GetAnticipationByIdOutput>> Handle(GetAnticipationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var anticipation = await anticipationRepository.GetByIdAsync(request.Id);

                if(anticipation is null)
                    return Result<GetAnticipationByIdOutput>.ResultFactory.NotFound();

                return Result<GetAnticipationByIdOutput>.ResultFactory.Success(
                    new GetAnticipationByIdOutput(
                        anticipation.Id,
                        anticipation.CreatorId,
                        anticipation.RequestedAmount,
                        anticipation.AppliedFee,
                        anticipation.NetAmount,
                        anticipation.RequestDate,
                        anticipation.AnalysisDate,
                        anticipation.Status
                    ));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao obter solicitações de antecipação para o Id {Id}", request.Id);
                return Result<GetAnticipationByIdOutput>.ResultFactory.InternalError($"Erro ao obter solicitações de antecipação: {ex.Message}");
            }
        }
    }
}
