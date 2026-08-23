using System.Text.Json.Serialization;

namespace TaskTracker.Models;

public class TaskItem
{
    public const string StatusTodo = "todo";
    public const string StatusInProgress = "in-progress";
    public const string StatusDone = "done";

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = StatusTodo;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
