using Microsoft.AspNetCore.Mvc;
using TaskMicroservice.Data;
using TaskMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
  private readonly TaskDbContext _context;

  public TaskController(TaskDbContext context)
  {
    _context = context;
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

    _context.Entry(taskItem).State = EntityState.Modified; //Tells EF Core that the item state has changed to modified

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException) //Handles concurrency gracefully by returning a conflict
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
  public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskItem createTaskItem)
  {
    var taskItem = new TaskItem
    {
      Id = Guid.NewGuid(),
      UserId = createTaskItem.UserId,
      Content = createTaskItem.Content,
      Reminder = createTaskItem.Reminder,
      IsCompleted = createTaskItem.IsCompleted
    };

    _context.Tasks.Add(taskItem);
    try
    {
      await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the task.", details = ex.Message });
    }

    return CreatedAtAction(nameof(GetTask), new { id = taskItem.Id }, taskItem); // 201 Created HTTP statuskode,
                                                                                 // tilføjer også en Location header til svaret, der peger på den nyoprettede ressource
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
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the task.", details = ex.Message });
    }

    return NoContent();
  }

  // GET; api/task/user/{userid}
  [HttpGet("user/{userId}")]
  public async Task<IActionResult>GetTasksForUser(string userId)
  {
    var tasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
    if (tasks == null)
    {
      return NotFound();
    }

    return Ok(tasks);
  }

  /// <summary>
  /// Check if task item exists
  /// </summary>
  /// <param name="id"> id of task item </param>
  /// <returns> true or false </returns>
  private bool TaskItemExists(Guid id)
  {
    return _context.Tasks.Any(e => e.Id == id);
  }
}
