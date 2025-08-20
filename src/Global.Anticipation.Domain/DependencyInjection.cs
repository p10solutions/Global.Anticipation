using Global.Anticipation.Domain.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Global.Anticipation.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AnticipationOptions>(configuration.GetSection("Anticipation"));

            return services;
        }
    }
}
