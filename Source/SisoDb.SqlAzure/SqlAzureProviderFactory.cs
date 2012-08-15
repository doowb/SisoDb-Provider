using SisoDb.Querying;
using SisoDb.SqlServer;

namespace SisoDb.SqlAzure
{
    public class SqlAzureProviderFactory : SqlServerProviderFactory
    {
        public SqlAzureProviderFactory()
            : base(StorageProviders.SqlAzure, new SqlAzureStatements()) { }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new SqlAzureQueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }
    }
}