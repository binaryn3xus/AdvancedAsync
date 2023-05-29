namespace AdvancedAsync.API.Jobs;

[DisallowConcurrentExecution]
public class MediumJob : IJob
{
    public static readonly JobKey JobKey = new("MediumRunningJob");
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
            await _sqlDataAccess.ExecuteAsync("MediumRunningProcedure", new { }, commandTimeout: 0, connectionId: "SqlServer", cancellationToken: context.CancellationToken);

            _logger.LogInformation("Completed Job {Key}", JobKey.Name);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Job {Key} was canceled.", JobKey.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Job: {Key}", JobKey.Name);
        }
    }
}
