namespace TaskTracker.Notifiers;

using TaskTracker.TaskStatusChanged;
using TaskTracker.Interfaces;

public class ConsoleNotifier : INotifier
{
    public void Notify(TaskStatusChangedArgs info)
    {
        Console.WriteLine(
            $"[Notify] \"{info.Title}\" is now {info.NewStatus}"
        );
    }
}