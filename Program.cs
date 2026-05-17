using TaskTracker.Store;
using TaskTracker.AuditLogger;
using TaskTracker.Requests;
using TaskTracker.TeamTask;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskStore>();
builder.Services.AddSingleton<AuditLogger>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/tasks", (TaskStore store) =>
    Results.Ok(store.GetAll()));

app.MapGet("/api/tasks/overdue", (TaskStore store) =>
    Results.Ok(store.GetAll().Where(task => task.IsOverdue)));


app.Run();