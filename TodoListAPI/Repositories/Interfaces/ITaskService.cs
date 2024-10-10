using MongoDB.Bson;
using TodoListAPI.Models;
using System.Collections.Generic;

namespace TodoListAPI.Repositories.Interfaces
{
    public interface ITaskService
    {
        (List<TaskItem> tasks, int totalCount) GetTasks(int page, int pageSize, string? status); 

        TaskItem GetTaskById(ObjectId id);

        void AddTask(TaskItem task);

        void UpdateTask(ObjectId id, TaskItem updatedTask);

        bool DeleteTask(ObjectId id);

        IEnumerable<TaskItem> GetTasksByStatus(string status);

        (double completedPercentage, double inProgressPercentage, double deletedPercentage) GetTaskPercentages();
    }
}
