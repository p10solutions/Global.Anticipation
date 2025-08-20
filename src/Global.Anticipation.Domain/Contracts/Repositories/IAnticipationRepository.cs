using Global.Anticipation.Domain.Entities;

namespace Global.Anticipation.Domain.Contracts.Repositories
{
    public interface IAnticipationRepository
    {
        Task<AnticipationEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<AnticipationEntity>> GetByCreatorIdAsync(Guid creatorId);
        Task<bool> HasPendingRequestForCreatorAsync(Guid creatorId);
        Task AddAsync(AnticipationEntity request);
        void Update(AnticipationEntity request);
    }
}
