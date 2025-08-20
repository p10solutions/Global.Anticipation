using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Domain.Models.Options;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation
{
    public class SimulateAnticipationHandler(IOptions<AnticipationOptions> feeOptions) : IRequestHandler<SimulateAnticipationInput, Result<SimulateAnticipationOutput>>
    {
        public async Task<Result<SimulateAnticipationOutput>> Handle(SimulateAnticipationInput request, CancellationToken cancellationToken)
        {
            var anticipation = new AnticipationEntity(Guid.NewGuid(), request.RequestedAmount, DateTime.Now);

            anticipation.ApplyFee(feeOptions.Value.FeeRate);

            return Result<SimulateAnticipationOutput>
                .ResultFactory
                .Success(new SimulateAnticipationOutput(anticipation.NetAmount));
        }
    }
}
