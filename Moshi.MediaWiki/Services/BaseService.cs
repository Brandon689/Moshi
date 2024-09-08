using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public abstract class BaseService
{
    protected readonly string ConnectionString;

    protected BaseService(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected SQLiteConnection CreateConnection()
    {
        return new SQLiteConnection(ConnectionString);
    }
}