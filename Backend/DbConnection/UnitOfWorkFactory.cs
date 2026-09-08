namespace Backend.DbConnection;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly NHibernate.ISession _session;

    public UnitOfWorkFactory(NHibernate.ISession session)
    {
        _session = session;
    }

    public IUnitOfWork Create()
    {
        return new UnitOfWork(_session);
    }
}
