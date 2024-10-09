using MongoDB.Bson;
using TodoListAPI.Models;

namespace TodoListAPI.Repositories.Interfaces
{
    public interface ITaskService
    {
        List<TaskItem> GetTasks();
        TaskItem GetTaskById(ObjectId id);
        void AddTask(TaskItem task);
        void UpdateTask(ObjectId id, TaskItem updatedTask);
        bool DeleteTask(ObjectId id);

        (IEnumerable<TaskItem> Tasks, int TotalCount) GetTasks(int page, int pageSize);
    }
}
