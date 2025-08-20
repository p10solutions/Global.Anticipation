using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Infra.Data.Persistence;
using Global.Anticipation.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Global.Anticipation.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(this IServiceCollection services)
        {
            services.AddScoped<IAnticipationRepository, AnticipationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<AnticipationContext>(options =>
            {
                options.UseInMemoryDatabase("AnticipationDb");
            });

            return services;
        }
    }
}
