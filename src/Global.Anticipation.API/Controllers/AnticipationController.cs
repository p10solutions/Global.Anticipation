using Asp.Versioning;
using Global.Anticipation.API.Controllers.Base;
using Global.Anticipation.API.Transport;
using Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId;
using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById;
using Global.Anticipation.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Global.Anticipation.API.Controllers
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/anticipations")]
    [ApiController]
    public class AnticipationController : ApiController
    {
        public AnticipationController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateAnticipationOutput))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAnticipationRequest request, CancellationToken cancellationToken)
        {
            var input = new CreateAnticipationInput(request.CreatorId, request.RequestedAmount, request.RequestDate);

            return await SendAsync(input, cancellationToken, System.Net.HttpStatusCode.Created);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAnticipationByIdOutput))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)        
           => await SendAsync(new GetAnticipationByIdQuery(id), cancellationToken);

        [HttpGet("/api/v{version:apiVersion}/creators/{creatorId:guid}/anticipations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAnticipationByCreatorIdOutput>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByCreatorId(Guid creatorId, CancellationToken cancellationToken)
            => await SendAsync(new GetAnticipationByCreatorIdQuery(creatorId), cancellationToken);

        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateAnticipationOutput))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
        {
            var input = new UpdateAnticipationInput(id, (Status)request.Status);

            return await SendAsync(input, cancellationToken);
        }

        [HttpGet("simulate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SimulateAnticipationOutput))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Simulate([FromQuery] decimal requestedAmount, CancellationToken cancellationToken)
        {
            var input = new SimulateAnticipationInput(requestedAmount);

            return await SendAsync(input, cancellationToken);
        }
    }
}
