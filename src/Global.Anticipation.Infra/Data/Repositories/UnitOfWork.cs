using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Infra.Data.Persistence;

namespace Global.Anticipation.Infra.Data.Repositories
{
    public class UnitOfWork(AnticipationContext _context): IUnitOfWork
    {
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
