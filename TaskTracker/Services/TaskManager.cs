using TaskTracker.Models;

namespace TaskTracker.Services;

/// <summary>Thrown for expected, user-facing errors (invalid input, missing task, etc.).</summary>
public sealed class TaskTrackerException(string message) : Exception(message);

/// <summary>Business logic for creating, updating and querying tasks.</summary>
public sealed class TaskManager(FileManager fileManager)
{
    public TaskItem Add(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            throw new TaskTrackerException("Task description cannot be empty.");

        var tasks = fileManager.LoadTasks();
        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Id = NextId(tasks),
            Description = description,
            Status = TaskItem.StatusTodo,
            CreatedAt = now,
            UpdatedAt = now,
        };
        tasks.Add(task);
        fileManager.SaveTasks(tasks);
        return task;
    }

    public TaskItem Update(long id, string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            throw new TaskTrackerException("Task description cannot be empty.");

        var tasks = fileManager.LoadTasks();
        var task = GetTaskOrThrow(tasks, id);
        task.Description = description;
        task.UpdatedAt = DateTime.UtcNow;
        fileManager.SaveTasks(tasks);
        return task;
    }

    public TaskItem Delete(long id)
    {
        var tasks = fileManager.LoadTasks();
        var task = GetTaskOrThrow(tasks, id);
        tasks.Remove(task);
        fileManager.SaveTasks(tasks);
        return task;
    }

    public TaskItem Mark(long id, string status)
    {
        var tasks = fileManager.LoadTasks();
        var task = GetTaskOrThrow(tasks, id);
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        fileManager.SaveTasks(tasks);
        return task;
    }

    public List<TaskItem> List(string? statusFilter)
    {
        var normalized = statusFilter?.ToLowerInvariant();
        if (normalized != null && normalized != TaskItem.StatusTodo &&
            normalized != TaskItem.StatusInProgress && normalized != TaskItem.StatusDone)
            throw new TaskTrackerException($"Invalid status '{statusFilter}'. Valid values: todo, in-progress, done.");

        var tasks = fileManager.LoadTasks();
        if (normalized != null)
            tasks = tasks.Where(t => t.Status == normalized).ToList();

        return tasks.OrderBy(t => t.Id).ToList();
    }

    static TaskItem GetTaskOrThrow(List<TaskItem> tasks, long id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
            throw new TaskTrackerException($"Task with ID {id} does not exist.");
        return task;
    }

    static long NextId(List<TaskItem> tasks) =>
        tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1;
}
