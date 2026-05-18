using TaskTracker.InMemory;
using TaskTracker.AuditLogger;
using TaskTracker.Requests;
using TaskTracker.TeamTask;
using TaskTracker.Interfaces;
using TaskTracker.Notifiers;
using TaskTracker.TaskStatuss;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskRepository,InMemoryTaskRepository>();
builder.Services.AddSingleton<AuditLogger>();
builder.Services.AddSingleton<INotifier,ConsoleNotifier>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/tasks", (ITaskRepository repo) =>
    Results.Ok(repo.GetAll()));

app.MapGet("/api/tasks/overdue", (ITaskRepository repo) =>
    Results.Ok(repo.GetAll().Where(task => task.IsOverdue)));

app.MapGet("/api/tasks/{id}", (int id, ITaskRepository repo) =>
{
    var task = repo.GetById(id);
    return task is null
        ? Results.NotFound(new { message = $"Task with id {id} not found." })
        : Results.Ok(task);
});

app.MapPost("/api/tasks", (CreateTaskRequest request, ITaskRepository repo, AuditLogger logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest(new { message = "Title is required and cannot be empty." });

    var task = new TeamTask { Title = request.Title, Description = request.Description, DueDate = request.DueDate };

    if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        task.Assign(request.AssignedTo);

    var auditNotifier = new AuditNotifier(logger);
    var consoleNotifier = new ConsoleNotifier();

    task.StatusChanged += (_, args) => auditNotifier.Notify(args);
    task.StatusChanged += (_, args) => consoleNotifier.Notify(args);
    task.StatusChanged += (_, args) =>
    {
        if (args.NewStatus == TaskStatuss.Done)
        {
            var who = args.AssignedTo ?? "someone";
            Console.WriteLine($"✓ \"{args.Title}\" marked as Done by {who}");
        }
    };

    repo.Add(task);
    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPatch("/api/tasks/{id}/assign", (int id, AssignRequest request, ITaskRepository repo) =>
{
    var task = repo.GetById(id);
    if (task is null)
        return Results.NotFound(new { message = $"Task with id {id} not found." });

    IAssignable assignable = task;
    assignable.Assign(request.User);
    return Results.NoContent();
});

app.MapPatch("/api/tasks/{id}/status", (int id, TransitionRequest request, ITaskRepository repo) =>
{
    var task = repo.GetById(id);
    if (task is null)
        return Results.NotFound(new { message = $"Task with id {id} not found." });

    if (request.NewStatus == task.Status)
        return Results.BadRequest(new { message = $"Task is already in status {task.Status}." });

    task.Transition(request.NewStatus);
    return Results.NoContent();
});

app.Run();