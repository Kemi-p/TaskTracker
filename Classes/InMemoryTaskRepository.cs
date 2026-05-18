namespace TaskTracker.InMemory;

using TaskTracker.TeamTask;
using TaskTracker.Interfaces;

public class InMemoryTaskRepository: ITaskRepository
{
    private readonly List<TeamTask> _tasks = new();

    public List<TeamTask> GetAll() => _tasks;

    public TeamTask? GetById(int id) => _tasks.FirstOrDefault(task => task.Id == id);

    public void Add(TeamTask task) => _tasks.Add(task);
}