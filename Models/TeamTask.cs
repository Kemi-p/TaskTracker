namespace TaskTracker.TeamTask;

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

    public void Assign(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("Assigned user cannot be null or whitespace.", nameof(user));

        AssignedTo = user;
        
    }

    public void Transition(TaskStatus newStatus)
    {
        if (newStatus == Status)return;
        Status = newStatus;
        
    }
}
