using FluentValidation;
using Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Commands.SimulateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Commands.UpdateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationByCreatorId;
using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Global.Anticipation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblyContaining<CreateAnticipationValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateAnticipationValidator>();
            services.AddValidatorsFromAssemblyContaining<SimulateAnticipationValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAnticipationByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAnticipationByCreatorIdValidator>();

            return services;
        }
    }
}
