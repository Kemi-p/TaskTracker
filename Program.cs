using Serilog;
using TaskTracker.InMemory;
using TaskTracker.AuditLogger;
using TaskTracker.Interfaces;
using TaskTracker.Notifiers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddSingleton<ITaskRepository,InMemoryTaskRepository>();
builder.Services.AddSingleton<AuditLogger>();
builder.Services.AddSingleton<INotifier,ConsoleNotifier>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTaskEndpoints();

app.Run();