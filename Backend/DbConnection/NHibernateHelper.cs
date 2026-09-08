using Backend.Domain;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Backend.DbConnection;

public static class NHibernateHelper
{
    private static ISessionFactory? _sessionFactory;
    private static string? _connectionString;

    public static void Configure(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static ISessionFactory SessionFactory
    {
        get
        {
            _sessionFactory ??= Fluently.Configure()
                                            .Database(
                                                PostgreSQLConfiguration.Standard
                                                    .ConnectionString(_connectionString)
                                                    .Dialect<NHibernate.Dialect.PostgreSQL83Dialect>()
                                                    .ShowSql()
                                            )
                                            .Mappings(m =>
                                                m.FluentMappings
                                                    .AddFromAssemblyOf<Organization>()
                                            )
                                            .BuildSessionFactory();

            return _sessionFactory;
        }
    }
}

