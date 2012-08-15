using System;
using SisoDb.SqlServer;

namespace SisoDb.SqlAzure
{
    [Serializable]
    public class SqlAzureConnectionInfo : SqlServerConnectionInfo
    {
        public SqlAzureConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public SqlAzureConnectionInfo(IConnectionString connectionString)
            : base(StorageProviders.SqlAzure, connectionString)
        { }
    }
}