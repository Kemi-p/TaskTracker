using TaskTracker.Requests;
using TaskTracker.TeamTask;
using TaskTracker.Interfaces;
using TaskTracker.Notifiers;
using TaskTracker.TaskStatuss;
using TaskTracker.AuditLogger;
using Microsoft.Extensions.Logging;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/tasks", (ITaskRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching all tasks");
            return Results.Ok(repo.GetAll());
        });

        endpoints.MapGet("/api/tasks/overdue", (ITaskRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching overdue tasks");
            return Results.Ok(repo.GetAll().Where(task => task.IsOverdue));
        });

        endpoints.MapGet("/api/tasks/{id}", (int id, ITaskRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching task {TaskId}", id);
            var task = repo.GetById(id);

            if (task is null)
            {
                logger.LogWarning("Task {TaskId} not found", id);
                return Results.NotFound(new { message = $"Task with id {id} not found." });
            }

            return Results.Ok(task);
        });

        endpoints.MapPost("/api/tasks", (CreateTaskRequest request, ITaskRepository repo, AuditLogger auditLogger, ILogger<Program> logger) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                logger.LogWarning("Create task rejected — title was empty");
                return Results.BadRequest(new { message = "Title is required and cannot be empty." });
            }

            var task = new TeamTask { Title = request.Title, Description = request.Description, DueDate = request.DueDate };

            if (!string.IsNullOrWhiteSpace(request.AssignedTo))
                task.Assign(request.AssignedTo);

            var auditNotifier = new AuditNotifier(auditLogger);
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
            logger.LogInformation("Task created: {TaskId} - {Title}", task.Id, task.Title);
            return Results.Created($"/api/tasks/{task.Id}", task);
        });

        endpoints.MapPatch("/api/tasks/{id}/assign", (int id, AssignRequest request, ITaskRepository repo, ILogger<Program> logger) =>
        {
            var task = repo.GetById(id);

            if (task is null)
            {
                logger.LogWarning("Assign failed — Task {TaskId} not found", id);
                return Results.NotFound(new { message = $"Task with id {id} not found." });
            }

            IAssignable assignable = task;
            assignable.Assign(request.User);
            logger.LogInformation("Task {TaskId} assigned to {User}", id, request.User);
            return Results.NoContent();
        });

        endpoints.MapPatch("/api/tasks/{id}/status", (int id, TransitionRequest request, ITaskRepository repo, ILogger<Program> logger) =>
        {
            var task = repo.GetById(id);

            if (task is null)
            {
                logger.LogWarning("Status transition failed — Task {TaskId} not found", id);
                return Results.NotFound(new { message = $"Task with id {id} not found." });
            }

            if (request.NewStatus == task.Status)
            {
                logger.LogWarning("Task {TaskId} is already in status {Status}", id, task.Status);
                return Results.BadRequest(new { message = $"Task is already in status {task.Status}." });
            }

            task.Transition(request.NewStatus);
            logger.LogInformation("Task {TaskId} transitioned to {NewStatus}", id, request.NewStatus);
            return Results.NoContent();
        });

        return endpoints;
    }
}