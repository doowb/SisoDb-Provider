using SisoDb.Dac;

namespace SisoDb.SqlAzure
{
	public class SqlAzureStatements : SqlStatementsBase
    {
    	public SqlAzureStatements()
            : base(typeof(SqlAzureStatements).Assembly, "Resources.SqlAzureStatements")
        {
        }
    }
}