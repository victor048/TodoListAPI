using MongoDB.Bson;
using TodoListAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoListAPI.Repositories.Interfaces
{
    public interface ITaskService
    {
        Task<(List<TaskItem> tasks, int totalCount)> GetTasksAsync(int page, int pageSize, string? status);
        Task<TaskItem> GetTaskByIdAsync(ObjectId id);
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(ObjectId id, TaskItem updatedTask);
        Task<bool> DeleteTaskAsync(ObjectId id);
        Task<List<TaskItem>> GetTasksByStatusAsync(string status);


        Task<(double completedPercentage, double inProgressPercentage, double deletedPercentage)> GetTaskPercentagesAsync();
    }
}
