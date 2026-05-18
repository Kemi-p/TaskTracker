namespace TaskTracker.Interfaces;
using TaskTracker.TeamTask;
using TaskTracker.TaskStatusChanged;
using TaskTracker.TaskStatuss;


public interface IAssignable
{
    void Assign(string user);
}

public interface ITransitionable
{
    TaskStatuss Status { get; }
    void Transition(TaskStatuss newStatus);
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

