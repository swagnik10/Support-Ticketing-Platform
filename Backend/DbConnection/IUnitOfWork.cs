namespace Backend.DbConnection;

public interface IUnitOfWork : IDisposable
{
    NHibernate.ISession Session { get; }
    void BeginTransaction();
    Task CommitAsync();
    Task RollbackAsync();
    bool HasActiveTransaction();
}
