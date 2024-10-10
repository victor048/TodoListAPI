using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TodoListAPI.Models;
using TodoListAPI.Repositories.Interfaces;

namespace TodoListAPI.Controllers
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
        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 10, string? status = null)
        {
            var (tasks, totalCount) = await _taskService.GetTasksAsync(page, pageSize, status);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = tasks.Select(t => new
            {
                Id = t.Id.ToString(),
                t.Title,
                t.Status
            });

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
                Tasks = response
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTaskById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var task = await _taskService.GetTaskByIdAsync(objectId);
            if (task == null)
            {
                return NotFound();
            }

            var response = new
            {
                Id = task.Id.ToString(),
                task.Title,
                task.Status
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<object>> AddTask([FromBody] CreateTaskDto newTaskDto)
        {
            if (string.IsNullOrWhiteSpace(newTaskDto.Title))
            {
                return BadRequest("Title cannot be empty.");
            }

            var newTask = new TaskItem
            {
                Title = newTaskDto.Title,
                Status = newTaskDto.Status
            };

            await _taskService.AddTaskAsync(newTask);

            var response = new
            {
                Id = newTask.Id.ToString(),
                newTask.Title,
                newTask.Status
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id.ToString() }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateTask(string id, [FromBody] CreateTaskDto updatedTaskDto)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var updatedTask = new TaskItem
            {
                Id = objectId,
                Title = updatedTaskDto.Title,
                Status = updatedTaskDto.Status
            };

            await _taskService.UpdateTaskAsync(objectId, updatedTask);

            return Ok(new
            {
                Id = updatedTask.Id.ToString(),
                updatedTask.Title,
                updatedTask.Status
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var deleted = await _taskService.DeleteTaskAsync(objectId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("completion-percentage")]
        public async Task<ActionResult> GetCompletionPercentage()
        {
            var (concluido, emAndamento, deletado) = await _taskService.GetTaskPercentagesAsync();
            return Ok(new
            {
                Concluido = concluido,
                EmAndamento = emAndamento,
                Deletado = deletado
            });
        }
    }
}
