// Services/TaskService.cs
using MongoDB.Driver;
using TodoList.Interfaces;
using TodoList.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Services
{
    public class TaskService : ITaskService
    {
        private readonly IMongoCollection<TaskItem> _taskCollection;

        public TaskService(IMongoDatabase database)
        {
            _taskCollection = database.GetCollection<TaskItem>("Tasks");
        }

        // Método para obter todas as tarefas
        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _taskCollection.Find(_ => true).ToListAsync();
        }

        // Método para obter uma tarefa por ID
        public async Task<TaskItem> GetTaskByIdAsync(string id)
        {
            return await _taskCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        // Método para criar uma nova tarefa
        public async Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
        {
            await _taskCollection.InsertOneAsync(taskItem);
            return taskItem;
        }

        // Método para atualizar uma tarefa existente
        public async Task<TaskItem> UpdateTaskAsync(TaskItem taskItem)
        {
            var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, taskItem.Id);
            var update = Builders<TaskItem>.Update
                .Set(t => t.Text, taskItem.Text)
                .Set(t => t.IsCompleted, taskItem.IsCompleted);

            await _taskCollection.UpdateOneAsync(filter, update);
            return taskItem;
        }

        // Método para deletar uma tarefa por ID
        public async Task<bool> DeleteTaskAsync(string id)
        {
            var result = await _taskCollection.DeleteOneAsync(t => t.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
