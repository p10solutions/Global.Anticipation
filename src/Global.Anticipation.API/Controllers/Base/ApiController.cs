using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Global.Anticipation.API.Controllers.Base
{
    [ExcludeFromCodeCoverage]
    public abstract class ApiController : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected ApiController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<IActionResult> SendAsync<T>(
            IRequest<Result<T>> request,
            CancellationToken cancellationToken,
            HttpStatusCode successStatusCode = HttpStatusCode.OK)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return GetObjectResultFromDomainResult(result, successStatusCode);
        }

        private IActionResult GetObjectResultFromDomainResult<T>(Result<T> result, HttpStatusCode successStatusCode)
        {
            return result.StatusResult switch
            {
                EStatusResult.Success =>
                    new ObjectResult(result.Response) { StatusCode = (int)successStatusCode },

                EStatusResult.BusinessError =>
                    BadRequest(result.Erros),

                EStatusResult.ExceptionError =>
                    StatusCode(StatusCodes.Status500InternalServerError, result.Erros),

                EStatusResult.NotFound =>
                    NotFound(result.Erros),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new[] { "Unhandled status result." })
            };
        }
    }
}
