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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }
    }
}