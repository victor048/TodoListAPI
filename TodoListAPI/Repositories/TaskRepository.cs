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

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            return await _taskCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<TaskItem>> GetTasksByStatusAsync(string? status)
        {
            var filter = Builders<TaskItem>.Filter.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter = Builders<TaskItem>.Filter.Eq(t => t.Status, status);
            }

            return await _taskCollection.Find(filter).ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(ObjectId id)
        {
            return await _taskCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await _taskCollection.InsertOneAsync(task);
        }

        public async Task UpdateTaskAsync(ObjectId id, TaskItem updatedTask)
        {
            await _taskCollection.ReplaceOneAsync(t => t.Id == id, updatedTask);
        }

        public async Task<bool> DeleteTaskAsync(ObjectId id)
        {
            var result = await _taskCollection.DeleteOneAsync(t => t.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
