using Microsoft.AspNetCore.Mvc;
using TodoList.Interfaces;
using TodoList.Models;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Text is required.");
            }

            var newTask = new TaskItem
            {
                Text = request.Text,
                IsCompleted = false
            };

            var createdTask = await _taskService.CreateTaskAsync(newTask);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] UpdateTaskRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Text is required.");
            }

            var taskToUpdate = await _taskService.GetTaskByIdAsync(id);
            if (taskToUpdate == null)
            {
                return NotFound();
            }

            taskToUpdate.Text = request.Text;
            taskToUpdate.IsCompleted = true;

            var updatedTask = await _taskService.UpdateTaskAsync(taskToUpdate);

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Endpoint para obter as estatísticas de conclusão de tarefas
        [HttpGet("completion-stats")]
        public async Task<IActionResult> GetCompletionStats()
        {
            var tasks = await _taskService.GetAllTasksAsync();  

            if (tasks == null || tasks.Count == 0)
            {
                return Ok(new { completedPercentage = 0, notCompletedPercentage = 100 });
            }

            var totalTasks = tasks.Count;  
            var completedTasks = tasks.Count(t => t.IsCompleted); 

            var completedPercentage = (double)completedTasks / totalTasks * 100;  
            var notCompletedPercentage = 100 - completedPercentage;  

            // Retorna os dados em formato JSON
            var stats = new
            {
                completedPercentage = completedPercentage,
                notCompletedPercentage = notCompletedPercentage
            };

            return Ok(stats);
        }
    }
}
