using FluentValidation;
using Global.Anticipation.Infra.CrossCutting.Results;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Global.Anticipation.Infra.CrossCutting.Validation
{
    [ExcludeFromCodeCoverage]
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
                return await next();

            var response = (TResponse)Activator.CreateInstance(typeof(TResponse), nonPublic: true)!;
            response.StatusResult = EStatusResult.BusinessError;
            response.Erros = failures.Select(ToMessage).ToArray(); 

            return response;
        }

        private static string ToMessage(ValidationFailure f)
        {
            return f.ErrorMessage;
        }
    }
}
