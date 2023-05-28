using AdvancedAsync.API.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<ShortJob>(opts => opts.WithIdentity(new JobKey(ShortJob.Key)).StoreDurably(true));
    q.AddJob<MediumJob>(opts => opts.WithIdentity(new JobKey(MediumJob.Key)).StoreDurably(true));
    q.AddJob<LongJob>(opts => opts.WithIdentity(new JobKey(LongJob.Key)).StoreDurably(true));
});
builder.Services.AddQuartzHostedService(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddEndpoints<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

//app.UseHttpsRedirection();

app.UseEndpoints<Program>();
app.Run();
