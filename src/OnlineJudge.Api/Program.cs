using Microsoft.EntityFrameworkCore;
using OnlineJudge.Core.Data;
using OnlineJudge.Core.Hub;
using OnlineJudge.Core.Models;
using OnlineJudge.Sandbox.Services;
using OnlineJudge.Sandbox.Workers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ISubmissionQueue, SubmissionQueue>();

builder.Services.AddDbContext<AppDbContext>(ops =>
{
    ops.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection") ?? throw new InvalidOperationException("Connection string 'PostgreSQLConnection' not found."));
});

builder.Services.AddTransient<ISandboxService, SandboxService>();
builder.Services.AddHostedService<JudgingWorker>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Applies all pending EF Core migrations to PostgreSQL container automatically
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while running database migrations: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.MapHub<SubmissionHub>("/r/submissions");

app.MapDefaultControllerRoute();

app.Run();
