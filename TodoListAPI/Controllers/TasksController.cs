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
        public ActionResult<object> GetTasks(int page = 1, int pageSize = 10)
        {
            var (tasks, totalCount) = _taskService.GetTasks(page, pageSize);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = tasks.Select(t => new
            {
                Id = t.Id.ToString(),
                t.Title,
                t.IsCompleted
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
        public ActionResult<object> GetTaskById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var task = _taskService.GetTaskById(objectId);
            if (task == null)
            {
                return NotFound();
            }

            var response = new
            {
                Id = task.Id.ToString(),
                task.Title,
                task.IsCompleted
            };

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<object> AddTask([FromBody] CreateTaskDto newTaskDto)
        {
            if (string.IsNullOrWhiteSpace(newTaskDto.Title))
            {
                return BadRequest("Title cannot be empty.");
            }

            var newTask = new TaskItem
            {
                Title = newTaskDto.Title,
                IsCompleted = newTaskDto.IsCompleted
            };

            _taskService.AddTask(newTask);

            var response = new
            {
                Id = newTask.Id.ToString(),
                newTask.Title,
                newTask.IsCompleted
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id.ToString() }, response);
        }

        [HttpPut("{id}")]
        public ActionResult<object> UpdateTask(string id, [FromBody] CreateTaskDto updatedTaskDto)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var updatedTask = new TaskItem
            {
                Id = objectId,
                Title = updatedTaskDto.Title,
                IsCompleted = updatedTaskDto.IsCompleted
            };

            _taskService.UpdateTask(objectId, updatedTask);

            return Ok(new
            {
                Id = updatedTask.Id.ToString(),
                updatedTask.Title,
                updatedTask.IsCompleted
            });
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTask(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var deleted = _taskService.DeleteTask(objectId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
