using AdvancedAsync.API.DbAccess;
using AdvancedAsync.API.Endpoints.Internal;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace AdvancedAsync.API.Endpoints
{
    public class SqlTestEndpoints : IEndpoints
    {
        public static void AddServices(IServiceCollection services) { }

        public static void DefineEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("api/ping", Ping)
                .WithName("Ping")
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

        internal static IResult ExecuteShortRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
        {
            sqlDataAccess.ExecuteLongRunningProcedureAsync("ShortRunningProcedure", new { }, "TestUser", 0, "SqlServer");
            return Results.Ok("Short Running Triggered");
        }

        internal static IResult ExecuteMediumRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
        {
            sqlDataAccess.ExecuteLongRunningProcedureAsync("MediumRunningProcedure", new { }, "TestUser", 0, "SqlServer");
            return Results.Ok("Medium Running Triggered");
        }

        internal static IResult ExecuteLongRunningProcedure([FromServices] ISqlDataAccess sqlDataAccess, [FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
        {
            sqlDataAccess.ExecuteLongRunningProcedureAsync("LongRunningProcedure", new { }, "TestUser", 0, "SqlServer");
            return Results.Ok("Long Running Triggered");
        }
    }
}
