namespace Captivlink.Infrastructure.Utility
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
