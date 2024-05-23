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
  private readonly DaprClient _daprClient;

  public TaskController(TaskDbContext context, DaprClient daprClient)
  {
    _context = context;
    _daprClient = daprClient;
  }

  // GET: api/Task
  [HttpGet]
  public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
  {
    return await _context.Tasks.ToListAsync();
  }

  // GET: api/Task/{guid-id}
  [HttpGet("{id}")]
  public async Task<ActionResult<TaskItem>> GetTask(Guid id)
  {
    var taskItem = await _context.Tasks.FindAsync(id);

    if (taskItem == null)
    {
      return NotFound(new { message = "Task not found." });
    }

    return taskItem;
  }

  // PUT: api/Task/{guid-id}
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(Guid id, TaskItem taskItem)
  {
    if (id != taskItem.Id)
    {
      return BadRequest(new { message = "Task ID mismatch." });
    }

    var existingTask = await _context.Tasks.FindAsync(id);

    if (existingTask == null)
    {
      return NotFound(new { message = "Task not found." });
    }

    _context.Entry(taskItem).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();

      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue)
      {
        await _daprClient.PublishEventAsync("reminderpubsub", "updatereminder", taskItem);
      }
      else if (existingTask.Reminder.HasValue && !taskItem.Reminder.HasValue)
      {
        await _daprClient.PublishEventAsync("reminderpubsub", "deletereminder", taskItem);
      }
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!TaskItemExists(id))
      {
        return NotFound(new { message = "Task not found." });
      }

      return Conflict(new { message = "Concurrency conflict: the task was modified by another user." });
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
      await _context.SaveChangesAsync();

      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue)
      {
        await _daprClient.PublishEventAsync("reminderpubsub", "newreminder", taskItem);
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
      return NotFound(new { message = "Task not found." });
    }

    _context.Tasks.Remove(taskItem);

    try
    {
      await _context.SaveChangesAsync();

      // Send besked til ReminderMicroservice via Dapr
      if (taskItem.Reminder.HasValue)
      {
        await _daprClient.PublishEventAsync("reminderpubsub", "deletereminder", taskItem);
      }
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the task.", details = ex.Message });
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
