namespace AdvancedAsync.API.Jobs;

[DisallowConcurrentExecution]
public class MediumJob : IJob
{
    public static string Key => "MediumRunningJob";
    private readonly ILogger<MediumJob> _logger;
    private readonly ISqlDataAccess _sqlDataAccess;

    public MediumJob(ILogger<MediumJob> logger, ISqlDataAccess sqlDataAccess)
    {
        _logger = logger;
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _sqlDataAccess.ExecuteAsync("MediumRunningProcedure", new { }, commandTimeout: 0, connectionId: "SqlServer");
            _logger.LogInformation("Completed Job {Key}", Key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Job: {Key}", Key);
        }
        return;
    }
}
