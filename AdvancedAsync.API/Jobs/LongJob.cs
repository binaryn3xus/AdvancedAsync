namespace AdvancedAsync.API.Jobs;

[DisallowConcurrentExecution]
public class LongJob : IJob
{
    public static string Key => "LongRunningJob";
    private readonly ILogger<LongJob> _logger;
    private readonly ISqlDataAccess _sqlDataAccess;

    public LongJob(ILogger<LongJob> logger, ISqlDataAccess sqlDataAccess)
    {
        _logger = logger;
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _sqlDataAccess.ExecuteAsync("LongRunningProcedure", new { }, commandTimeout: 0, connectionId: "SqlServer");
            _logger.LogInformation("Completed Job {Key}", Key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Job: {Key}", Key);
        }
        return;
    }
}
