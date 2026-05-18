namespace TaskTracker.TaskStatusChanged;
using TaskTracker.TaskStatuss;
public class TaskStatusChangedArgs: EventArgs
{
    public int TaskId { get; init; }
    public required string Title { get; init; }
    public TaskStatuss OldStatus { get; init; }
    public TaskStatuss NewStatus { get; init; }
    public string? AssignedTo { get; init; }

}