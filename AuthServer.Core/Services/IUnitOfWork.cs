namespace AuthServer.Core.Services
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
