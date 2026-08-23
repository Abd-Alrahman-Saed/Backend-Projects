# Task Tracker CLI

A simple command-line application to track and manage your to-do list, built with C# / .NET using only the standard library.

Based on the [Task Tracker project](https://roadmap.sh/projects/task-tracker) from roadmap.sh.

## Features

- Add, update, and delete tasks
- Mark tasks as `in-progress` or `done`
- List all tasks, or filter by status (`todo`, `in-progress`, `done`)
- Tasks are persisted in a `tasks.json` file in the current directory (created automatically on first use)

Each task is stored with the following properties:

| Property      | Description                                  |
| ------------- | -------------------------------------------- |
| `id`          | Unique identifier for the task               |
| `description` | Short description of the task                |
| `status`      | `todo`, `in-progress`, or `done`             |
| `createdAt`   | Date/time when the task was created (UTC)    |
| `updatedAt`   | Date/time when the task was last updated (UTC) |

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) 10.0 or later

## Usage

Run the app from the project directory:

```bash
dotnet run -- <command> [arguments]
```

Or build once and use the compiled binary directly:

```bash
dotnet build
./bin/Debug/net10.0/"1. Task Tracker" <command> [arguments]
```

### Commands

```bash
# Adding a new task
task-cli add "Buy groceries"
# Output: Task added successfully (ID: 1)

# Updating and deleting tasks
task-cli update 1 "Buy groceries and cook dinner"
task-cli delete 1

# Marking a task as in progress or done
task-cli mark-in-progress 1
task-cli mark-done 1

# Listing all tasks
task-cli list

# Listing tasks by status
task-cli list done
task-cli list todo
task-cli list in-progress
```

### Example output

```
ID    STATUS         CREATED AT            UPDATED AT            DESCRIPTION
1     done           2026-08-23 22:18:58   2026-08-23 22:19:48   Buy groceries and cook dinner
2     in-progress    2026-08-23 22:19:00   2026-08-23 22:19:47   Finish project report
3     todo           2026-08-23 22:19:01   2026-08-23 22:19:01   Read a book
```

## Error handling

The CLI validates input and exits with a non-zero status code on failure:

- Missing or empty arguments
- Unknown commands or invalid status filters
- Non-numeric, zero/negative, or non-existent task IDs
- A corrupted `tasks.json` file

## Project structure

```
TaskTracker/
├── Program.cs
├── Models/
│   └── TaskItem.cs
├── Services/
│   ├── TaskManager.cs
│   └── FileManager.cs
└── tasks.json
```
