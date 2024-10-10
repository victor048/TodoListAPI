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
        public double GetCompletionPercentage()
        {
            var tasks = _taskRepository.GetTasks();
            if (tasks.Count == 0)
                return 0;

            var completedTasks = tasks.Count(t => t.Status == "Completed");
            return (double)completedTasks / tasks.Count * 100;
        }
    }
}
