using MongoDB.Bson;
using Moq;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;
using TodoListAPI.Services;

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
        public void GetTaskById_ShouldReturnTask_WhenTaskExists()
        {
            var taskId = ObjectId.GenerateNewId();
            var task = new TaskItem { Id = taskId, Title = "Task 1", Status = "Pending" };

            _taskRepositoryMock.Setup(repo => repo.GetTaskById(taskId)).Returns(task);

            var result = _taskService.GetTaskById(taskId);

            Assert.NotNull(result);
            Assert.Equal(taskId.ToString(), result.Id.ToString());
        }

        // Teste para obter uma tarefa por ID não existente
        [Fact]
        public void GetTaskById_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.GetTaskById(taskId)).Returns((TaskItem)null);

            var result = _taskService.GetTaskById(taskId);

            Assert.Null(result);
        }

        // Teste para adicionar uma nova tarefa
        [Fact]
        public void AddTask_ShouldAddNewTask()
        {
            // Arrange
            var newTask = new TaskItem { Title = "New Task", Status = "Pending" };

            
            _taskService.AddTask(newTask);

            _taskRepositoryMock.Verify(repo => repo.AddTask(It.IsAny<TaskItem>()), Times.Once);
        }

        // Teste para adicionar uma tarefa com título vazio (deve lançar exceção)
        [Fact]
        public void AddTask_ShouldThrowException_WhenTitleIsEmpty()
        {
            var newTask = new TaskItem { Title = "", Status = "Pending" };

            var exception = Assert.Throws<ArgumentException>(() => _taskService.AddTask(newTask));
            Assert.Equal("Title cannot be empty", exception.Message);
        }

        // Teste para atualizar uma tarefa existente
        [Fact]
        public void UpdateTask_ShouldUpdateExistingTask()
        {
            var taskId = ObjectId.GenerateNewId();
            var updatedTask = new TaskItem { Id = taskId, Title = "Updated Task", Status = "Completed" };

            _taskService.UpdateTask(taskId, updatedTask);

            _taskRepositoryMock.Verify(repo => repo.UpdateTask(taskId, updatedTask), Times.Once);
        }

        // Teste para tentar atualizar uma tarefa que não existe
        [Fact]
        public void UpdateTask_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var taskId = ObjectId.GenerateNewId();
            var updatedTask = new TaskItem { Id = taskId, Title = "Non-existing Task", Status = "Completed" };

            _taskRepositoryMock.Setup(repo => repo.UpdateTask(taskId, updatedTask)).Throws(new Exception("Task not found"));

            var exception = Assert.Throws<Exception>(() => _taskService.UpdateTask(taskId, updatedTask));
            Assert.Equal("Task not found", exception.Message);
        }

        // Teste para excluir uma tarefa existente
        [Fact]
        public void DeleteTask_ShouldRemoveTask()
        {
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.DeleteTask(taskId)).Returns(true);

            var result = _taskService.DeleteTask(taskId);

            Assert.True(result);
            _taskRepositoryMock.Verify(repo => repo.DeleteTask(taskId), Times.Once);
        }

        // Teste para tentar excluir uma tarefa que não existe
        [Fact]
        public void DeleteTask_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            var taskId = ObjectId.GenerateNewId();

            _taskRepositoryMock.Setup(repo => repo.DeleteTask(taskId)).Returns(false);

            var result = _taskService.DeleteTask(taskId);

            Assert.False(result);
            _taskRepositoryMock.Verify(repo => repo.DeleteTask(taskId), Times.Once);
        }

        // Teste para obter tarefas filtradas por status
        [Fact]
        public void GetTasksByStatus_ShouldReturnFilteredTasks()
        {
            var status = "Pending";
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 1", Status = "Pending" },
                new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 2", Status = "Completed" }
            };

            _taskRepositoryMock.Setup(repo => repo.GetTasksByStatus(status)).Returns(new List<TaskItem> { tasks[0] });

            var result = _taskService.GetTasksByStatus(status);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Task 1", result.First().Title);
        }

        // Teste para obter tarefas quando o status é nulo
        [Fact]
        public void GetTasksByStatus_ShouldReturnAllTasks_WhenStatusIsNull()
        {
            
            var tasks = new List<TaskItem>
    {
        new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 1", Status = "Pending" },
        new TaskItem { Id = ObjectId.GenerateNewId(), Title = "Task 2", Status = "Completed" }
    };

            _taskRepositoryMock.Setup(repo => repo.GetTasks()).Returns(tasks);

            // Act
            var result = _taskService.GetTasksByStatus(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
