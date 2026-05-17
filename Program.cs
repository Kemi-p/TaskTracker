using TaskTracker.AuditLogger;
using TaskTracker.TeamTask;
using TaskTracker.TaskStatusChanged;
// var builder = WebApplication.CreateBuilder(args);

// var app = builder.Build();

// app.Run();

var logger = new AuditLogger();

var task = new TeamTask { Title = "Fix login bug" };
task.Assign("Alice");

task.StatusChanged += logger.OnStatusChanged;

task.StatusChanged += (sender, args) =>
    Console.WriteLine($"[Notify] \"{args.Title}\" is now {args.NewStatus}");

task.StatusChanged += (sender, args) =>
{
    if (args.NewStatus == TaskStatuss.Done)
    {
        var who = args.AssignedTo ?? "someone";
        Console.WriteLine($"✓ \"{args.Title}\" marked as Done by {who}");
    }
};

task.Transition(TaskStatuss.InProgress);
task.Transition(TaskStatuss.InReview);
task.Transition(TaskStatuss.Done);

Console.WriteLine("\n--- Audit Log ---");
foreach (var entry in logger.Log)
    Console.WriteLine(entry);


