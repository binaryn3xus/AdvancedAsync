namespace AdvancedAsync.API.Jobs;

[DisallowConcurrentExecution]
public class ShortJob : IJob
{
    public static readonly JobKey JobKey = new("ShortRunningJob");
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
            var seconds = context.MergedJobDataMap.GetInt("Seconds");
            _logger.LogInformation("Seconds: {Seconds}", seconds);

            var sampleData = context.MergedJobDataMap.GetString("SampleData");
            _logger.LogInformation("SampleData: {SampleData}", sampleData);

            await _sqlDataAccess.ExecuteAsync("ShortRunningProcedure", new { Seconds = seconds, SampleData = sampleData }, commandTimeout: 0, connectionId: "SqlServer", cancellationToken: context.CancellationToken);

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
