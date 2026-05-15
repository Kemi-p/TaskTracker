namespace TaskTracker.AuditLogger;
using TaskTracker.TaskStatusChanged;

public class AuditLogger
{
   public  List<string> Log {get;} =new();

    public void OnStatusChanged(object? sender, TaskStatusChangedArgs args)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{args.TaskId} " +
                    $"\"{args.Title}\": {args.OldStatus} → {args.NewStatus}";
        Log.Add(entry);
    }
}