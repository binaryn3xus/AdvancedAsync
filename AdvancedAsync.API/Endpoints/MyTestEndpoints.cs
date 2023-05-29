namespace AdvancedAsync.API.Endpoints;

public class SqlTestEndpoints : IEndpoints
{
    public static void AddServices(IServiceCollection services) { }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/ping", Ping).WithName("Ping").WithOpenApi();
        app.MapGet("api/jobs/running", GetRunningJobs).WithName("RunningJobs").WithOpenApi();
        app.MapGet("api/short", ExecuteShortRunningProcedure).WithName("ExecuteShortRunningProcedure").WithOpenApi();
        app.MapGet("api/short/stop", StopShortRunningProcedure).WithName("CancelShortRunningProcedure").WithOpenApi();
        app.MapGet("api/medium", ExecuteMediumRunningProcedure).WithName("ExecuteMediumRunningProcedure").WithOpenApi();
        app.MapGet("api/long", ExecuteLongRunningProcedure).WithName("ExecuteLongRunningProcedure").WithOpenApi();
    }

    internal static IResult Ping(CancellationToken cancellationToken = default)
    {
        return Results.Ok("Pong");
    }

    internal static async Task<IResult> GetRunningJobs([FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var runningJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
        return Results.Ok(runningJobs.Select(x => new
        {
            JobKeyName = x.JobDetail.Key.Name,
            ElapsedTime = DateTime.UtcNow - x.FireTimeUtc,
            CancellationRequested = x.CancellationToken.IsCancellationRequested
        }));
    }

    internal static async Task<IResult> ExecuteShortRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        var job = JobBuilder.Create<ShortJob>().WithIdentity(ShortJob.JobKey)
            .UsingJobData("Seconds", 180)
            .UsingJobData("SampleData", "MasterChief")
            .Build();

        var trigger = TriggerBuilder.Create().WithIdentity($"{ShortJob.JobKey.Name}-trigger", "Short").StartNow().Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);

        //await scheduler.TriggerJob(ShortJob.JobKey, cancellationToken);
        return Results.Ok("Short Running Triggered");
    }

    internal static async Task<IResult> StopShortRunningProcedure([FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var runningJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
        var job = runningJobs.FirstOrDefault(x => x.JobDetail.Key.Name == ShortJob.JobKey.Name);
        if (job != null)
        {
            var jobToken = job.CancellationToken;
            if (jobToken.CanBeCanceled)
            {
                var result = await scheduler.Interrupt(job.JobDetail.Key, job.CancellationToken);
                return (result) ? Results.Ok("Short Running Job Canceled") : Results.BadRequest("Short Running Job Could Not Be Canceled");
            }
        }
        return Results.BadRequest("No Short Running Job Found Running");
    }

    internal static async Task<IResult> ExecuteMediumRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(MediumJob.JobKey, cancellationToken);
        return Results.Ok("Medium Running Triggered");
    }

    internal static async Task<IResult> ExecuteLongRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(LongJob.JobKey, cancellationToken);
        return Results.Ok("Long Running Triggered");
    }
}
