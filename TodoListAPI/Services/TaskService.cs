using MongoDB.Bson;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoListAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public (List<TaskItem> tasks, int totalCount) GetTasks(int page, int pageSize, string? status)
        {
            var query = _taskRepository.GetTasks();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalCount = query.Count();

            var tasks = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return (tasks, totalCount);
        }

        public TaskItem GetTaskById(ObjectId id)
        {
            return _taskRepository.GetTaskById(id);
        }

        public void AddTask(TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            task.Status = task.Status ?? "Iniciada";

            _taskRepository.AddTask(task);
        }


        public void UpdateTask(ObjectId id, TaskItem updatedTask)
        {
            if (string.IsNullOrWhiteSpace(updatedTask.Title))
                throw new ArgumentException("Title cannot be empty");

            _taskRepository.UpdateTask(id, updatedTask);
        }

        public bool DeleteTask(ObjectId id)
        {
            return _taskRepository.DeleteTask(id);
        }

        public IEnumerable<TaskItem> GetTasksByStatus(string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return _taskRepository.GetTasks();
            }

            return _taskRepository.GetTasksByStatus(status);
        }
        public (double completedPercentage, double inProgressPercentage, double deletedPercentage) GetTaskPercentages()
        {
            var tasks = _taskRepository.GetTasks();
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
