using Global.Anticipation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Global.Anticipation.Infra.Data.Persistence
{
    public class AnticipationContext(DbContextOptions<AnticipationContext> options) : DbContext(options)
    {
        public DbSet<AnticipationEntity> Anticipation { get; set; }
    }
}
