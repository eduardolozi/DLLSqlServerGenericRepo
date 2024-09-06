using Microsoft.Data.SqlClient;

namespace Infra.Context;

public class DbContext
{
    private readonly string _connectionString;
    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection CreateSqlConnection()
    {
        return new SqlConnection(_connectionString);
    }
}