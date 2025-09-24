using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HmctsBackend.Data;
using HmctsBackend.Models;

namespace HmctsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Create a task
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
        {
            try
            {
                var validStatuses = new[] { "To do", "Doing", "Done" };
                if (!validStatuses.Contains(task.Status))
                    return BadRequest(new { Message = $"Invalid status. Allowed values: {string.Join(", ", validStatuses)}" });

                if (task.DueDate <= DateTime.UtcNow)
                    return BadRequest(new { Message = $"Invalid due date. Tasks must be scheduled for a future date." });

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while creating task"
                );
            }
        }

        // Retrieve all tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            try
            {
                return Ok(await _context.Tasks.ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while retrieving tasks"
                );
            }
        }

        // Retrieve a task by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskById(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });
                return Ok(task);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while retrieving task"
                );
            }
        }

        // Retrieve tasks by Status
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest(new { Message = "Status must be provided." });

                var tasks = await _context.Tasks
                                        .Where(t => t.Status == status)
                                        .ToListAsync();

                if (!tasks.Any())
                    return NotFound(new { Message = $"No tasks found with status '{status}'." });

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while retrieving tasks"
                );
            }
        }

        // Update the status of a task
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return BadRequest(new { Message = "Status cannot be empty." });
                }

                var validStatuses = new[] { "To do", "Doing", "Done" };
                if (!validStatuses.Contains(status))
                    return BadRequest(new { Message = $"Invalid status. Allowed values: {string.Join(", ", validStatuses)}" });

                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

                task.Status = status;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while updating task"
                );
            }
        }

        // Delete a task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound(new { Message = $"Task with ID {id} not found." });

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error while deleting task"
                );
            }
        }
    }
}