namespace AdvancedAsync.API.Jobs;

[DisallowConcurrentExecution]
public class ShortJob : IJob
{
    public static string Key => "ShortRunningJob";
    private readonly ILogger<ShortJob> _logger;
    private readonly ISqlDataAccess _sqlDataAccess;

    public ShortJob(ILogger<ShortJob> logger, ISqlDataAccess sqlDataAccess)
    {
        _logger = logger;
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _sqlDataAccess.ExecuteAsync("ShortRunningProcedure", new { }, commandTimeout: 0, connectionId: "SqlServer");
            _logger.LogInformation("Completed Job {Key}", Key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Job: {Key}", Key);
        }
        return;
    }
}