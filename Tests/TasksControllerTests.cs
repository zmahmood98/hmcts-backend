using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HmctsBackend.Controllers;
using HmctsBackend.Data;
using HmctsBackend.Models;
using NUnit.Framework;

namespace HmctsBackend.Tests
{
    [TestFixture]
    public class TasksControllerTests
    {
        private TasksController _controller = null!;
        private AppDbContext _context = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new TasksController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region GET /api/tasks

        [Test]
        public async Task GetTasks_WhenNoTasks_ShouldReturnEmptyList()
        {
            // Act
            var result = await _controller.GetTasks();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var tasks = okResult!.Value as IEnumerable<TaskItem>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks!.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetTasks_WhenTasksExist_ShouldReturnAllTasks()
        {
            // Arrange
            var testTasks = new List<TaskItem>
            {
                new() { Title = "Task 1", Status = "To do", DueDate = DateTime.Now.AddDays(1) },
                new() { Title = "Task 2", Status = "Doing", DueDate = DateTime.Now.AddDays(2) }
            };
            _context.Tasks.AddRange(testTasks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTasks();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var tasks = okResult!.Value as IEnumerable<TaskItem>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks!.Count(), Is.EqualTo(2));
        }

        #endregion

        #region GET /api/tasks/{id}

        [Test]
        public async Task GetTaskById_WhenTaskExists_ShouldReturnTask()
        {
            // Arrange
            var testTask = new TaskItem { Title = "Test Task", Status = "To do", DueDate = DateTime.Now.AddDays(1) };
            _context.Tasks.Add(testTask);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTaskById(testTask.Id);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var task = okResult!.Value as TaskItem;
            Assert.That(task, Is.Not.Null);
            Assert.That(task!.Title, Is.EqualTo("Test Task"));
        }

        [Test]
        public async Task GetTaskById_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.GetTaskById(999);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        #endregion

        #region GET /api/tasks/status/{status}

        [Test]
        [TestCase("To do")]
        [TestCase("Doing")]
        [TestCase("Done")]
        public async Task GetTasksByStatus_WhenTasksWithStatusExist_ShouldReturnMatchingTasks(string status)
        {
            // Arrange
            var testTasks = new List<TaskItem>
            {
                new() { Title = "Task 1", Status = status, DueDate = DateTime.Now.AddDays(1) },
                new() { Title = "Task 2", Status = status, DueDate = DateTime.Now.AddDays(2) },
                new() { Title = "Task 3", Status = "Different Status", DueDate = DateTime.Now.AddDays(3) }
            };
            _context.Tasks.AddRange(testTasks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTasksByStatus(status);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var tasks = okResult!.Value as IEnumerable<TaskItem>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks!.Count(), Is.EqualTo(2));
            Assert.That(tasks.All(t => t.Status == status), Is.True);
        }

        [Test]
        public async Task GetTasksByStatus_WhenNoTasksWithStatus_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.GetTasksByStatus("NonExistent");

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public async Task GetTasksByStatus_WhenStatusIsInvalid_ShouldReturnBadRequest(string invalidStatus)
        {
            // Act
            var result = await _controller.GetTasksByStatus(invalidStatus);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        #endregion

        #region POST /api/tasks

        [Test]
        public async Task CreateTask_WhenValidTask_ShouldReturnCreatedResult()
        {
            // Arrange
            var newTask = new TaskItem
            {
                Title = "New Task",
                Description = "Test Description",
                Status = "To do",
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act
            var result = await _controller.CreateTask(newTask);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var task = createdResult!.Value as TaskItem;
            Assert.That(task, Is.Not.Null);
            Assert.That(task!.Title, Is.EqualTo("New Task"));
            Assert.That(task.Id, Is.GreaterThan(0));
        }

        [Test]
        [TestCase("Invalid Status")]
        [TestCase("")]
        [TestCase("Pending")]
        public async Task CreateTask_WhenInvalidStatus_ShouldReturnBadRequest(string invalidStatus)
        {
            // Arrange
            var newTask = new TaskItem
            {
                Title = "New Task",
                Status = invalidStatus,
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act
            var result = await _controller.CreateTask(newTask);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        [Test]
        public async Task CreateTask_WhenInvalidDueDate_ShouldReturnBadRequest()
        {
            // Arrange
            var newTask = new TaskItem
            {
                Title = "New Task",
                Status = "Doing",
                DueDate = DateTime.Now.AddDays(-7)
            };

            // Act
            var result = await _controller.CreateTask(newTask);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [TestCase("To do")]
        [TestCase("Doing")]
        [TestCase("Done")]
        public async Task CreateTask_WhenValidStatus_ShouldCreateTask(string validStatus)
        {
            // Arrange
            var newTask = new TaskItem
            {
                Title = "New Task",
                Status = validStatus,
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act
            var result = await _controller.CreateTask(newTask);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var task = createdResult!.Value as TaskItem;
            Assert.That(task!.Status, Is.EqualTo(validStatus));
        }

        #endregion

        #region PUT /api/tasks/{id}/status

        [Test]
        public async Task UpdateStatus_WhenTaskExistsAndValidStatus_ShouldReturnNoContent()
        {
            // Arrange
            var testTask = new TaskItem { Title = "Test Task", Status = "To do", DueDate = DateTime.Now.AddDays(1) };
            _context.Tasks.Add(testTask);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateStatus(testTask.Id, "Done");

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            
            // Verify the task was updated
            var updatedTask = await _context.Tasks.FindAsync(testTask.Id);
            Assert.That(updatedTask!.Status, Is.EqualTo("Done"));
        }

        [Test]
        public async Task UpdateStatus_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.UpdateStatus(999, "Done");

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        [TestCase("Invalid Status")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public async Task UpdateStatus_WhenInvalidStatus_ShouldReturnBadRequest(string invalidStatus)
        {
            // Arrange
            var testTask = new TaskItem { Title = "Test Task", Status = "To do", DueDate = DateTime.Now.AddDays(1) };
            _context.Tasks.Add(testTask);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateStatus(testTask.Id, invalidStatus);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        #endregion

        #region DELETE /api/tasks/{id}

        [Test]
        public async Task DeleteTask_WhenTaskExists_ShouldReturnNoContentAndDeleteTask()
        {
            // Arrange
            var testTask = new TaskItem { Title = "Test Task", Status = "To do", DueDate = DateTime.Now.AddDays(1) };
            _context.Tasks.Add(testTask);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteTask(testTask.Id);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            
            // Verify the task was deleted
            var deletedTask = await _context.Tasks.FindAsync(testTask.Id);
            Assert.That(deletedTask, Is.Null);
        }

        [Test]
        public async Task DeleteTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.DeleteTask(999);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        #endregion
    }
}

