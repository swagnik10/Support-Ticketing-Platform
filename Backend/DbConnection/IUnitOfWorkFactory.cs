namespace Backend.DbConnection;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}