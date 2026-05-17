namespace TaskTracker.Requests;

using TaskTracker.TeamTask;

public record CreateTaskRequest(string Title, string? Description, string? AssignedTo, DateTime? DueDate);
public record AssignRequest(string User);
public record TransitionRequest(TaskStatuss NewStatus);