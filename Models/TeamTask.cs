namespace TaskTracker.TeamTask;

using TaskTracker.TaskStatusChanged;
public enum TaskStatus
{
    Backlog,
    InProgress,
    InReview,
    Done
}

public class TeamTask
{
    private static int _nextId = 1;

    public int Id { get; } = _nextId++;
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? AssignedTo { get; private set; }
    public DateTime? DueDate { get; init; }
    public TaskStatus Status { get; private set; } = TaskStatus.Backlog;

    public bool IsOverdue
    {
        get
        {
            if (DueDate.HasValue)
            {
                if (DueDate.Value.Date < DateTime.Today)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public string Label => AssignedTo ?? "Unassigned";

    public void Assign(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("Assigned user cannot be null or whitespace.", nameof(user));

        AssignedTo = user;

    }
    public event EventHandler<TaskStatusChangedArgs>? StatusChanged;

    public void Transition(TaskStatus newStatus)
    {
        if (newStatus == Status) return;
        var oldStatus = Status;
        Status = newStatus;

        StatusChanged?.Invoke(this, new TaskStatusChangedArgs
        {
            TaskId = Id,
            Title = Title,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            AssignedTo = AssignedTo
        });

    }

}
