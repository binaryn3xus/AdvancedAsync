using AdvancedAsync.API.Jobs;
using Quartz.Impl.Matchers;

namespace AdvancedAsync.API.Endpoints;

public class SqlTestEndpoints : IEndpoints
{
    public static void AddServices(IServiceCollection services) { }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/ping", Ping)
            .WithName("Ping")
            .WithOpenApi();

        app.MapGet("api/jobs/running", GetRunningJobs)
            .WithName("RunningJobs")
            .WithOpenApi();

        app.MapGet("api/short", ExecuteShortRunningProcedure)
            .WithName("ExecuteShortRunningProcedure")
            .WithOpenApi();

        app.MapGet("api/medium", ExecuteMediumRunningProcedure)
            .WithName("ExecuteMediumRunningProcedure")
            .WithOpenApi();

        app.MapGet("api/long", ExecuteLongRunningProcedure)
            .WithName("ExecuteLongRunningProcedure")
            .WithOpenApi();
    }

    internal static IResult Ping(CancellationToken cancellationToken = default)
    {
        return Results.Ok("Pong");
    }

    internal static async Task<IResult> GetRunningJobs([FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var runningJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
        return Results.Ok(runningJobs.Select(x => new {
            JobKeyName = x.JobDetail.Key.Name,
            ElapsedTime = DateTime.UtcNow - x.FireTimeUtc
        }));
    }

    internal static async Task<IResult> ExecuteShortRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(new JobKey(ShortJob.Key), cancellationToken);
        return Results.Ok("Short Running Triggered");
    }

    internal static async Task<IResult> ExecuteMediumRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(new JobKey(MediumJob.Key), cancellationToken);
        //sqlDataAccess.ExecuteLongRunningProcedureAsync("MediumRunningProcedure", new { }, "TestUser", 0, "SqlServer");
        return Results.Ok("Medium Running Triggered");
    }

    internal static async Task<IResult> ExecuteLongRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(new JobKey(LongJob.Key), cancellationToken);
        //sqlDataAccess.ExecuteLongRunningProcedureAsync("LongRunningProcedure", new { }, "TestUser", 0, "SqlServer");
        return Results.Ok("Long Running Triggered");
    }
}
