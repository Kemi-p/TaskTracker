namespace TaskTracker.TaskStatusChanged;
using TaskTracker.TeamTask;
public class TaskStatusChangedArgs: EventArgs
{
    public int TaskId { get; init; }
    public required string Title { get; init; }
    public TaskStatus OldStatus { get; init; }
    public TaskStatus NewStatus { get; init; }
    public string? AssignedTo { get; init; }

}