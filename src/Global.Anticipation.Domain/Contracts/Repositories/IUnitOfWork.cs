namespace Global.Anticipation.Domain.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
