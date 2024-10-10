using MongoDB.Bson;
using TodoListAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoListAPI.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(ObjectId id);
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(ObjectId id, TaskItem updatedTask);
        Task<bool> DeleteTaskAsync(ObjectId id);
        Task<List<TaskItem>> GetTasksByStatusAsync(string status);
    }
}
