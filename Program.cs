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

app.MapGet("/api/tasks/{id}", (int id, TaskStore store) =>
{
    var task = store.GetById(id);
    return task is null
        ? Results.NotFound(new { message = $"Task with id {id} not found." })
        : Results.Ok(task);
});

app.MapPost("/api/tasks", (CreateTaskRequest request, TaskStore store, AuditLogger logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest(new { message = "Title is required and cannot be empty." });

    var task = new TeamTask
    {
        Title = request.Title,
        Description = request.Description,
        DueDate = request.DueDate
    };

    if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        task.Assign(request.AssignedTo);

    task.StatusChanged += logger.OnStatusChanged;

    store.Add(task);

    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPatch("/api/tasks/{id}/assign", (int id, AssignRequest request, TaskStore store) =>
{
    var task = store.GetById(id);
    if (task is null)
        return Results.NotFound(new { message = $"Task with id {id} not found." });

    task.Assign(request.User);
    return Results.NoContent();
});

app.MapPatch("/api/tasks/{id}/status", (int id, TransitionRequest request, TaskStore store) =>
{
    var task = store.GetById(id);
    if (task is null)
        return Results.NotFound(new { message = $"Task with id {id} not found." });

    if (request.NewStatus == task.Status)
        return Results.BadRequest(new { message = $"Task is already in status {task.Status}." });

    task.Transition(request.NewStatus);
    return Results.NoContent();
});
app.Run();