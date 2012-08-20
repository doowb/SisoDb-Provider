using System;
using System.Data;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.SqlAzure
{
    public class SqlAzureServerClient : SqlServerClient
    {
        public SqlAzureServerClient(IAdoDriver driver, ISisoConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(driver, connectionInfo, connectionManager, sqlStatements)
        { }

        public override void EnsureNewDb()
        {
            ConnectionManager.ReleaseAllConnections();

            DropDbIfItExists();
            WithConnection(cn =>
            {
                OnExecuteNonQuery(cn, SqlStatements.GetSql("CreateDatabase").Inject(ConnectionInfo.DbName));
            });

            WithClientConnection(cn => 
            {
                OnInitializeSysTables(cn);
                OnInitializeSysTypes(cn);
            });
        }

        public override void DropDbIfItExists()
        {
            ConnectionManager.ReleaseAllConnections();
            if (DbExists())
            {
                WithConnection(cn =>
                {
                    OnExecuteNonQuery(cn, SqlStatements.GetSql("DropDatabase").Inject(ConnectionInfo.DbName));
                });
            }
        }

        public override bool DbExists()
        {
            return WithClientConnection(cn => OnExecuteScalar<int>(cn, SqlStatements.GetSql("DatabaseExists"), new DacParameter(DbSchemas.Parameters.DbNameParamPrefix, ConnectionInfo.DbName)) > 0);
        }

        protected void WithClientConnection(Action<IDbConnection> cnConsumer)
        {
            IDbConnection cn = null;

            try
            {
                cn = ConnectionManager.OpenClientConnection(ConnectionInfo);
                cnConsumer.Invoke(cn);
            }
            finally
            {
                ConnectionManager.ReleaseClientConnection(cn);
            }
        }

        protected T WithClientConnection<T>(Func<IDbConnection, T> cnConsumer)
        {
            T result;
            IDbConnection cn = null;

            try
            {
                cn = ConnectionManager.OpenClientConnection(ConnectionInfo);
                result = cnConsumer.Invoke(cn);
            }
            finally
            {
                ConnectionManager.ReleaseClientConnection(cn);
            }

            return result;
        }
    }
}
