using MongoDB.Bson;
using Moq;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;
using TodoListAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TodoListAPI.Tests
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepositoryMock.Object);
        }

        // Teste para obter uma tarefa por ID existente
        [Fact]
        public async Task GetTaskById_ShouldReturnTask_WhenTaskExists()
        {
            var taskId = ObjectId.GenerateNewId();
            var task = new TaskItem { Id = taskId, Title = "Task 1", Status = "Pending" };

            _taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);

            var result = await _taskService.GetTaskByIdAsync(taskId);

            Assert.NotNull(result);
            Assert.Equal(taskId.ToString(), result.Id.ToString());
        }

        // Teste para obter uma tarefa por ID não existente
        [Fact]
        public async Task GetTaskById_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            var result = await _taskService.GetTaskByIdAsync(taskId);

            Assert.Null(result);
        }

        // Teste para adicionar uma nova tarefa
        [Fact]
        public async Task AddTask_ShouldAddNewTask()
        {
            // Arrange
            var newTask = new TaskItem { Title = "New Task", Status = "Pending" };

            await _taskService.AddTaskAsync(newTask);

            _taskRepositoryMock.Verify(repo => repo.AddTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        // Teste para adicionar uma tarefa com título vazio (deve lançar exceção)
        [Fact]
        public async Task AddTask_ShouldThrowException_WhenTitleIsEmpty()
        {
            var newTask = new TaskItem { Title = "", Status = "Pending" };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _taskService.AddTaskAsync(newTask));
            Assert.Equal("Title cannot be empty", exception.Message);
        }

        // Teste para atualizar uma tarefa existente
        [Fact]
        public async Task UpdateTask_ShouldUpdateExistingTask()
        {
            var taskId = ObjectId.GenerateNewId();
            var updatedTask = new TaskItem { Id = taskId, Title = "Updated Task", Status = "Completed" };

            await _taskService.UpdateTaskAsync(taskId, updatedTask);

            _taskRepositoryMock.Verify(repo => repo.UpdateTaskAsync(taskId, updatedTask), Times.Once);
        }

        // Teste para tentar atualizar uma tarefa que não existe
        [Fact]
        public async Task UpdateTask_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var taskId = ObjectId.GenerateNewId();
            var updatedTask = new TaskItem { Id = taskId, Title = "Non-existing Task", Status = "Completed" };

            _taskRepositoryMock.Setup(repo => repo.UpdateTaskAsync(taskId, updatedTask)).ThrowsAsync(new Exception("Task not found"));

            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.UpdateTaskAsync(taskId, updatedTask));
            Assert.Equal("Task not found", exception.Message);
        }

        // Teste para excluir uma tarefa existente
        [Fact]
        public async Task DeleteTask_ShouldRemoveTask()
        {
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId)).ReturnsAsync(true);

            var result = await _taskService.DeleteTaskAsync(taskId);

            Assert.True(result);
            _taskRepositoryMock.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        // Teste para tentar excluir uma tarefa que não existe
        [Fact]
        public async Task DeleteTask_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId)).ReturnsAsync(false);

            var result = await _taskService.DeleteTaskAsync(taskId);

            Assert.False(result);
            _taskRepositoryMock.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        // Teste para obter tarefas filtradas por status
        [Fact]
        public async Task GetTasksByStatus_ShouldReturnFilteredTasks()
        {
            var status = "Pending";
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 1", Status = "Pending" },
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 2", Status = "Completed" }
            };

            _taskRepositoryMock.Setup(repo => repo.GetTasksByStatusAsync(status)).ReturnsAsync(new List<TaskItem> { tasks[0] });

            var result = await _taskService.GetTasksByStatusAsync(status);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Task 1", result.First().Title);
        }

        // Teste para obter tarefas quando o status é nulo
        [Fact]
        public async Task GetTasksByStatus_ShouldReturnAllTasks_WhenStatusIsNull()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 1", Status = "Pending" },
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 2", Status = "Completed" }
            };

            _taskRepositoryMock.Setup(repo => repo.GetTasksAsync()).ReturnsAsync(tasks);

            var result = await _taskService.GetTasksByStatusAsync(null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        // Teste para verificar porcentagem quando todas as tarefas estão concluídas
        [Fact]
        public async Task GetTaskPercentages_ShouldReturn100PercentCompleted_WhenAllTasksCompleted()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem { Status = "Concluido" },
                new TaskItem { Status = "Concluido" },
                new TaskItem { Status = "Concluido" }
            };

            _taskRepositoryMock.Setup(repo => repo.GetTasksAsync()).ReturnsAsync(tasks);

            var (completedPercentage, inProgressPercentage, deletedPercentage) = await _taskService.GetTaskPercentagesAsync();

            Assert.Equal(100, completedPercentage);   // 100% concluídas
            Assert.Equal(0, inProgressPercentage);    // 0% em andamento
            Assert.Equal(0, deletedPercentage);       // 0% deletadas
        }
    }
}
