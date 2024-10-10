using MongoDB.Bson;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;

namespace TodoListAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<(List<TaskItem> tasks, int totalCount)> GetTasksAsync(int page, int pageSize, string? status)
        {
            var tasks = await _taskRepository.GetTasksAsync();

            if (!string.IsNullOrEmpty(status))
            {
                tasks = tasks.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalCount = tasks.Count();
            var pagedTasks = tasks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return (pagedTasks, totalCount);
        }

        public async Task<TaskItem> GetTaskByIdAsync(ObjectId id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            task.Status ??= "Iniciada"; // Atribui "Iniciada" se Status for null

            await _taskRepository.AddTaskAsync(task);
        }

        public async Task UpdateTaskAsync(ObjectId id, TaskItem updatedTask)
        {
            if (string.IsNullOrWhiteSpace(updatedTask.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            await _taskRepository.UpdateTaskAsync(id, updatedTask);
        }

        public async Task<bool> DeleteTaskAsync(ObjectId id)
        {
            return await _taskRepository.DeleteTaskAsync(id);
        }

        public async Task<List<TaskItem>> GetTasksByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return await _taskRepository.GetTasksAsync();
            }

            return await _taskRepository.GetTasksByStatusAsync(status);
        }

        public async Task<(double completedPercentage, double inProgressPercentage, double deletedPercentage)> GetTaskPercentagesAsync()
        {
            var tasks = await _taskRepository.GetTasksAsync();
            if (tasks == null || !tasks.Any())
            {
                return (0, 0, 0);
            }

            var totalTasks = tasks.Count();
            var completedTasks = tasks.Count(t => t.Status == "Concluido");
            var inProgressTasks = tasks.Count(t => t.Status == "Em Andamento");
            var deletedTasks = tasks.Count(t => t.Status == "Deletado");

            var completedPercentage = (double)completedTasks / totalTasks * 100;
            var inProgressPercentage = (double)inProgressTasks / totalTasks * 100;
            var deletedPercentage = (double)deletedTasks / totalTasks * 100;

            return (completedPercentage, inProgressPercentage, deletedPercentage);
        }
    }
}
