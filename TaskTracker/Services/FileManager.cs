using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Services;

/// <summary>Handles reading and writing the tasks JSON file on disk.</summary>
public sealed class FileManager(string fileName = "tasks.json")
{
    public string FileName { get; } = fileName;

    public List<TaskItem> LoadTasks()
    {
        if (!File.Exists(FileName))
            return [];

        var json = File.ReadAllText(FileName);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? [];
    }

    public void SaveTasks(List<TaskItem> tasks) =>
        File.WriteAllText(FileName, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));
}
