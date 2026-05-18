namespace TaskTracker.Notifiers;

using TaskTracker.TaskStatusChanged;
using TaskTracker.Interfaces;

public class ConsoleNotifier : INotifier
{
    public void Notify(TaskStatusChangedArgs args)
    {
        Console.WriteLine(
            $"[Notify] \"{args.Title}\" is now {args.NewStatus}"
        );
    }
}