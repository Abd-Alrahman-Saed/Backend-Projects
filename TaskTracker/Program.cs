// Task Tracker CLI - https://roadmap.sh/projects/task-tracker
using System.Globalization;
using TaskTracker.Models;
using TaskTracker.Services;

var fileManager = new FileManager();
var taskManager = new TaskManager(fileManager);

try
{
    return args.Length == 0 ? ShowUsage() : Run(args);
}
catch (TaskTrackerException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (System.Text.Json.JsonException)
{
    Console.Error.WriteLine($"Error: '{fileManager.FileName}' is corrupted and could not be read.");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

int Run(string[] args)
{
    switch (args[0].ToLowerInvariant())
    {
        case "add":
            if (RequireArgs(args, 2, "Usage: task-cli add \"description\"")) return AddTask(args[1]);
            break;

        case "update":
            if (RequireArgs(args, 3, "Usage: task-cli update <id> \"description\"")) return UpdateTask(args[1], args[2]);
            break;

        case "delete":
            if (RequireArgs(args, 2, "Usage: task-cli delete <id>")) return DeleteTask(args[1]);
            break;

        case "mark-in-progress":
            if (RequireArgs(args, 2, "Usage: task-cli mark-in-progress <id>")) return MarkTask(args[1], TaskItem.StatusInProgress);
            break;

        case "mark-done":
            if (RequireArgs(args, 2, "Usage: task-cli mark-done <id>")) return MarkTask(args[1], TaskItem.StatusDone);
            break;

        case "list":
            return ListTasks(args.Length > 1 ? args[1] : null);

        default:
            return UnknownCommand(args[0]);
    }
    return 1;
}

bool RequireArgs(string[] args, int required, string usage)
{
    if (args.Length >= required) return true;

    Console.Error.WriteLine("Error: Missing required argument(s).");
    Console.Error.WriteLine(usage);
    return false;
}

int UnknownCommand(string command)
{
    Console.Error.WriteLine($"Error: Unknown command '{command}'.");
    ShowUsage();
    return 1;
}

int ShowUsage()
{
    Console.WriteLine("Task Tracker CLI");
    Console.WriteLine();
    Console.WriteLine("Usage: task-cli <command> [arguments]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  add \"description\"               Add a new task");
    Console.WriteLine("  update <id> \"description\"       Update an existing task's description");
    Console.WriteLine("  delete <id>                     Delete a task");
    Console.WriteLine("  mark-in-progress <id>           Mark a task as in progress");
    Console.WriteLine("  mark-done <id>                  Mark a task as done");
    Console.WriteLine("  list                            List all tasks");
    Console.WriteLine("  list <todo|in-progress|done>    List tasks filtered by status");
    return args.Length == 0 ? 1 : 0;
}

int AddTask(string description)
{
    var task = taskManager.Add(description);
    Console.WriteLine($"Task added successfully (ID: {task.Id})");
    return 0;
}

int UpdateTask(string idInput, string description)
{
    var task = taskManager.Update(ParseId(idInput), description);
    Console.WriteLine($"Task updated successfully (ID: {task.Id})");
    return 0;
}

int DeleteTask(string idInput)
{
    var task = taskManager.Delete(ParseId(idInput));
    Console.WriteLine($"Task deleted successfully (ID: {task.Id})");
    return 0;
}

int MarkTask(string idInput, string status)
{
    var task = taskManager.Mark(ParseId(idInput), status);
    Console.WriteLine(status == TaskItem.StatusDone
        ? $"Task marked as done (ID: {task.Id})"
        : $"Task marked as in progress (ID: {task.Id})");
    return 0;
}

int ListTasks(string? statusFilter)
{
    var tasks = taskManager.List(statusFilter);
    if (tasks.Count == 0)
    {
        Console.WriteLine(statusFilter == null
            ? "No tasks found."
            : $"No tasks found with status '{statusFilter.ToLowerInvariant()}'.");
        return 0;
    }

    Console.WriteLine($"{Pad("ID", 4)}  {Pad("STATUS", 13)}  {Pad("CREATED AT", 20)}  {Pad("UPDATED AT", 20)}  DESCRIPTION");
    foreach (var task in tasks)
    {
        Console.WriteLine(
            $"{Pad(task.Id.ToString(), 4)}  {Pad(task.Status, 13)}  " +
            $"{Pad(Local(task.CreatedAt), 20)}  {Pad(Local(task.UpdatedAt), 20)}  {task.Description}");
    }
    return 0;

    static string Pad(string value, int width) =>
        value.Length <= width ? value.PadRight(width) : value[..(width - 3)] + "...";

    static string Local(DateTime utc) =>
        utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}

long ParseId(string idInput)
{
    if (!long.TryParse(idInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) || id < 1)
        throw new TaskTrackerException($"'{idInput}' is not a valid task ID.");
    return id;
}
