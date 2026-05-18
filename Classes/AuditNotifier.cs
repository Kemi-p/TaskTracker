namespace TaskTracker.Notifiers;

using TaskTracker.AuditLogger;
using TaskTracker.TaskStatusChanged;
using TaskTracker.Interfaces;

public class AuditNotifier : INotifier
{
    private readonly AuditLogger _logger;

    public AuditNotifier(AuditLogger logger)
    {
        _logger = logger;
    }

    public void Notify(TaskStatusChangedArgs info)
    {
        _logger.OnStatusChanged(null, info);
    }
}