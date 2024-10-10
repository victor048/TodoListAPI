using MongoDB.Bson;
using TodoListAPI.Models;

namespace TodoListAPI.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskItem> GetTasks();
        List<TaskItem> GetTasksByStatus(string? status);
        TaskItem GetTaskById(ObjectId id);
        void AddTask(TaskItem task);
        void UpdateTask(ObjectId id, TaskItem updatedTask);
        bool DeleteTask(ObjectId id);
    }
}
