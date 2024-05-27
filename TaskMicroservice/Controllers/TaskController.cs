using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMicroservice.Data;
using TaskMicroservice.Models;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
  private readonly TaskDbContext _context;
  private readonly ILogger<TaskController> _logger;
  private readonly DaprClient _daprClient;

  public TaskController(TaskDbContext context, ILogger<TaskController> logger, DaprClient daprClient)
  {
    _context = context;
    _logger = logger;
    _daprClient = daprClient;
  }


  // GET: api/Task/{guid-id}
  [HttpGet("{id}")]
  public async Task<ActionResult<TaskItem>> GetTask(Guid id)
  {
    var taskItem = await _context.Tasks.FindAsync(id);

    if (taskItem == null)
    {
      return NotFound("Task not found.");
    }

    return taskItem;
  }


  // PUT: api/Task/{guid-id}
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(Guid id, TaskItem taskItem)
  {
    if (id != taskItem.Id)
    {
      return BadRequest("Task ID mismatch.");
    }

    var existingTask = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

    if (existingTask == null)
    {
      return NotFound("Task not found.");
    }

    _context.Entry(taskItem).State = EntityState.Modified;

    try
    {


      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue && taskItem.IsCompleted == false)
      {
        await _daprClient.PublishEventAsync("pubsub", "updatereminder", taskItem);

      }

      if (existingTask.Reminder.HasValue && !taskItem.Reminder.HasValue || taskItem.IsCompleted)
      {
        await _daprClient.PublishEventAsync("pubsub", "deletereminder", taskItem);

      }
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!TaskItemExists(id))
      {
        return NotFound("Task not found.");
      }

      return Conflict("Concurrency conflict: the task was modified by another user.");
    }

    return NoContent();
  }

  // POST: api/Task
  [HttpPost]
  public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskItem createTaskDto)
  {
    var taskItem = new TaskItem
    {
      Id = Guid.NewGuid(),
      UserId = createTaskDto.UserId,
      Content = createTaskDto.Content,
      Reminder = createTaskDto.Reminder,
      IsCompleted = createTaskDto.IsCompleted,
      UserEmail = createTaskDto.UserEmail // Inkluder brugerens email, så reminder kan sendes til vedkommende
    };

    _context.Tasks.Add(taskItem);
    try
    {

      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue)
      {
        _logger.LogInformation("Publishing new reminder event");
        await _daprClient.PublishEventAsync("pubsub", "newreminder", taskItem);
        await _context.SaveChangesAsync();
      }

    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the task.", details = ex.Message });
    }

    return CreatedAtAction(nameof(GetTask), new { id = taskItem.Id }, taskItem);
  }

  // DELETE: api/Task/{guid-id}
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(Guid id)
  {
    var taskItem = await _context.Tasks.FindAsync(id);

    if (taskItem == null)
    {
      return NotFound("Task not found.");
    }

    _context.Tasks.Remove(taskItem);


    try
    {
      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue)
      {
        await _daprClient.PublishEventAsync("reminderpubsub", "deletereminder", taskItem);
        await _context.SaveChangesAsync();
        return Ok();
      }

    }
    catch (Exception)
    {
      return StatusCode(500, "An error occurred while deleting the task.");
    }

    return NoContent();
  }

  // GET; api/task/user/{userid}
  [HttpGet("user/{userId}")]
  public async Task<IActionResult> GetTasksForUser(string userId)
  {
    var tasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
    if (tasks == null)
    {
      return NotFound();
    }

    return Ok(tasks);
  }

  private bool TaskItemExists(Guid id)
  {
    return _context.Tasks.Any(e => e.Id == id);
  }
}
