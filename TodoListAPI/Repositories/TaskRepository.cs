using MongoDB.Bson;
using MongoDB.Driver;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;

namespace TodoListAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<TaskItem> _taskCollection;

        public TaskRepository(IMongoDatabase database)
        {
            _taskCollection = database.GetCollection<TaskItem>("Tasks");
        }

        public List<TaskItem> GetTasks()
        {
            return _taskCollection.Find(new BsonDocument()).ToList();
        }

        public List<TaskItem> GetTasksByStatus(string? status)
        {
            var filter = Builders<TaskItem>.Filter.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter = Builders<TaskItem>.Filter.Eq(t => t.Status, status);
            }

            return _taskCollection.Find(filter).ToList();
        }

        public TaskItem GetTaskById(ObjectId id)
        {
            return _taskCollection.Find(t => t.Id == id).FirstOrDefault();
        }

        public void AddTask(TaskItem task)
        {
            _taskCollection.InsertOne(task);
        }

        public void UpdateTask(ObjectId id, TaskItem updatedTask)
        {
            _taskCollection.ReplaceOne(t => t.Id == id, updatedTask);
        }

        public bool DeleteTask(ObjectId id)
        {
            var result = _taskCollection.DeleteOne(t => t.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
