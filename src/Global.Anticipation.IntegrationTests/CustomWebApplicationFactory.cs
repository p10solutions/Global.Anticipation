using Global.Anticipation.Infra.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Global.Anticipation.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AnticipationContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                services.AddDbContext<AnticipationContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryDbForTesting");
                });
            });
        }
    }
}