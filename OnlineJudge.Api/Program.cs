using Microsoft.EntityFrameworkCore;
using OnlineJudge.Core.Data;
using OnlineJudge.Core.Models;
using OnlineJudge.Sandbox.Services;
using OnlineJudge.Sandbox.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ISubmissionQueue, SubmissionQueue>();

builder.Services.AddDbContext<AppDbContext>(ops =>
{
    ops.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection") ?? throw new InvalidOperationException("Connection string 'PostgreSQLConnection' not found."));
});

builder.Services.AddTransient<ISandboxService, SandboxService>();
builder.Services.AddHostedService<JudgingWorker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapDefaultControllerRoute();

app.Run();
