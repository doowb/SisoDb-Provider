using SisoDb.SqlServer;

namespace SisoDb.SqlAzure
{
    public class SqlAzureSession : SqlServerSession
    {
        internal SqlAzureSession(ISisoDatabase db) : base(db)
        {}
    }
}