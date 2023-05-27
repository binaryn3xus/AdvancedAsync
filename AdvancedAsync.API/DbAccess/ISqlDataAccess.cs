using System.Data;
using System.Data.SqlClient;

namespace AdvancedAsync.API.DbAccess
{
    public interface ISqlDataAccess
    {
        SqlConnection GetConnection(string connectionId = "SqlServer");
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string query, U parameters, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task<int> ModifyDataAsync<T>(string sqlQuery, T parameters, string connectionId = "SqlServer", IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync<U>(string sqlQuery, U parameters, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task<T> LoadFirstOrDefaultAsync<T, U>(string query, U parameters, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task ExecuteAsync<T>(string storedProcedure, T parameters, int? commandTimeout = null, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> ExecuteWithResultsAsync<T, U>(string storedProcedure, U parameters, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task StartSqlServerJobAsync(string jobName, string connectionId = "SqlServer", CancellationToken cancellationToken = default);
        Task ExecuteLongRunningProcedureAsync<T>(string storedProcedure, T parameters, string? executedBy = null, int? commandTimeout = 0, string connectionId = "SqlServer");
    }
}
