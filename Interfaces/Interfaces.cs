namespace TaskTracker.Interfaces;
using TaskTracker.TeamTask;
using TaskTracker.TaskStatusChanged;


public interface IAssignable
{
    void Assign(string user);
}

public interface ITransitionable
{
    TaskStatus Status { get; }
    void Transition(TaskStatus newStatus);
}

public interface ISchedulable
{
    DateTime? DueDate { get; }
    bool IsOverdue { get; }
}

public interface ITaskRepository
{
    List<TeamTask> GetAll();
    TeamTask? GetById(int id);
    void Add(TeamTask task);
}

public interface INotifier
{
    void Notify(TaskStatusChangedArgs args);
}