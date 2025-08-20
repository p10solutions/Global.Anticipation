using Global.Anticipation.Domain.Contracts.Repositories;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Global.Anticipation.Infra.Data.Repositories
{
    public class AnticipationRepository(AnticipationContext context) : IAnticipationRepository
    {
        public async Task AddAsync(AnticipationEntity entity)
        {
            await context.Anticipation.AddAsync(entity);
        }

        public async Task<AnticipationEntity?> GetByIdAsync(Guid id)
        {
            return await context.Anticipation.FindAsync(id);
        }

        public async Task<IEnumerable<AnticipationEntity>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await context.Anticipation
                .Where(r => r.CreatorId == creatorId)
                .ToListAsync();
        }

        public async Task<bool> HasPendingRequestForCreatorAsync(Guid creatorId)
        {
            return await context.Anticipation
                .AnyAsync(r => r.CreatorId == creatorId && r.Status == Status.Pending);
        }

        public async void Update(AnticipationEntity request)
        {
            context.Anticipation.Update(request);
        }
    }
}
