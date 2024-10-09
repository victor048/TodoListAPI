using MongoDB.Bson;
using MongoDB.Driver;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;

namespace TodoListAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMongoCollection<TaskItem> _taskCollection;

        public TaskService(ITaskRepository taskRepository, IMongoDatabase database)
        {
            _taskRepository = taskRepository;
            _taskCollection = database.GetCollection<TaskItem>("tasks");
        }

        public List<TaskItem> GetTasks()
        {
            return _taskRepository.GetTasks();
        }

        public TaskItem GetTaskById(ObjectId id)
        {
            return _taskRepository.GetTaskById(id);
        }

        public void AddTask(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            _taskRepository.AddTask(task);
        }

        public void UpdateTask(ObjectId id, TaskItem updatedTask)
        {
            _taskRepository.UpdateTask(id, updatedTask);
        }

        public bool DeleteTask(ObjectId id)
        {
            return _taskRepository.DeleteTask(id);
        }

        public (IEnumerable<TaskItem> Tasks, int TotalCount) GetTasks(int page, int pageSize)
        {
            var totalCount = _taskCollection.CountDocuments(new BsonDocument());
            var tasks = _taskCollection.Find(new BsonDocument())
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return (tasks, (int)totalCount);
        }
    }

}
