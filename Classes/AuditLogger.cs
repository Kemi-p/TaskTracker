namespace TaskTracker.AuditLogger;
using TaskTracker.TaskStatusChanged;

public class AuditLogger
{
   public  List<string> Log {get;} =new();

    public void OnStatusChanged(object? sender, TaskStatusChangedArgs info)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{info.TaskId} " +
                    $"\"{info.Title}\": {info.OldStatus} → {info.NewStatus}";
        Log.Add(entry);
    }
}