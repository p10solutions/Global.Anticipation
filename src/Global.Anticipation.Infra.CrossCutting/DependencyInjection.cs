using Global.Anticipation.Infra.CrossCutting.Validation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Global.Anticipation.Infra.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCrossCutting(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}
