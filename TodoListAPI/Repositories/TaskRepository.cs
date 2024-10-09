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
            _taskCollection = database.GetCollection<TaskItem>("tasks");
        }

        public List<TaskItem> GetTasks()
        {
            return _taskCollection.Find(task => true).ToList();
        }

        public TaskItem GetTaskById(ObjectId id)
        {
            return _taskCollection.Find(task => task.Id == id).FirstOrDefault();
        }

        public void AddTask(TaskItem task)
        {
            _taskCollection.InsertOne(task);
        }

        public void UpdateTask(ObjectId id, TaskItem updatedTask)
        {
            updatedTask.Id = id;
            _taskCollection.ReplaceOne(task => task.Id == id, updatedTask);
        }

        public bool DeleteTask(ObjectId id)
        {
            var result = _taskCollection.DeleteOne(task => task.Id == id);
            return result.DeletedCount > 0;
        }
    }

}
