namespace AdvancedAsync.API.DbAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private const string DefaultConnectionId = "SqlServer";
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public SqlDataAccess(ILogger<SqlDataAccess> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public SqlConnection GetConnection(string connectionId = DefaultConnectionId)
    {
        return new SqlConnection(_configuration.GetConnectionString(connectionId));
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string sqlQuery, U parameters, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        var cmdDefinition = new CommandDefinition(sqlQuery, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken);
        return await connection.QueryAsync<T>(cmdDefinition);
    }

    public async Task<int> ModifyDataAsync<T>(string sqlQuery, T parameters, string connectionId = DefaultConnectionId, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        var cmdDefinition = new CommandDefinition(sqlQuery, parameters, transaction, commandType: CommandType.Text, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(cmdDefinition);
    }

    public async Task<int> GetCountAsync<U>(string sqlQuery, U parameters, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        var cmdDefinition = new CommandDefinition(sqlQuery, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(cmdDefinition);
    }

    public async Task<T> LoadFirstOrDefaultAsync<T, U>(string sqlQuery, U parameters, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        var cmdDefinition = new CommandDefinition(sqlQuery, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<T>(cmdDefinition);
    }

    public async Task ExecuteAsync<T>(string storedProcedure, T parameters, int? commandTimeout = null, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
            await connection.ExecuteAsync(storedProcedure, parameters, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Execute Stored Procedure: '{StoredProcedure}' | Parameters: {Parameters}", storedProcedure, parameters);
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task<IEnumerable<T>> ExecuteWithResultsAsync<T, U>(string storedProcedure, U parameters, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task StartSqlServerJobAsync(string jobName, string connectionId = DefaultConnectionId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
        var cmdDefinition = new CommandDefinition("EXEC msdb.dbo.sp_start_job @JobName", new { JobName = jobName }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(cmdDefinition);
    }

    public Task ExecuteLongRunningProcedureAsync<T>(string storedProcedure, T parameters, string? executedBy = null, int? commandTimeout = 0, string connectionId = DefaultConnectionId)
    {
        Task.Run(async () =>
        {
            var guid = Guid.NewGuid();
            _logger.LogInformation("Starting Long Running Background Procedure: Id: '{GuidId}' Name: '{StoredProcedureName}' with Parameters: {Parameters}", guid, storedProcedure, parameters);
            try
            {
                await ExecuteAsync(storedProcedure, parameters, commandTimeout: 0);
                _logger.LogInformation("Completed Long Running Background Procedure: Id: '{GuidId}' Name: '{StoredProcedureName}'", guid, storedProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Execute Long Running Procedure: {StoredProcedureName}", storedProcedure);
            }
        });
        return Task.CompletedTask;
    }
}