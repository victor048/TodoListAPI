using TodoList.Models;

namespace TodoList.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllTasksAsync();  
        Task<TaskItem> GetTaskByIdAsync(string id);  
        Task<TaskItem> CreateTaskAsync(TaskItem taskItem);  
        Task<TaskItem> UpdateTaskAsync(TaskItem taskItem);  
        Task<bool> DeleteTaskAsync(string id);  
    }
}
