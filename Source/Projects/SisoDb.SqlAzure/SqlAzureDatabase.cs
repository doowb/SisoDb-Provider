using SisoDb.SqlServer;

namespace SisoDb.SqlAzure
{
    public class SqlAzureDatabase : SqlServerDatabase
    {
        public SqlAzureDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

    	protected override DbSession CreateSession()
    	{
            return new SqlAzureSession(this);
    	}
    }
}