using Dapr;
using Microsoft.AspNetCore.Mvc;
using ReminderMicroservice_.Models;
using ReminderMicroservice_.Services.Interfaces;

namespace ReminderMicroservice_.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReminderController : ControllerBase
{
  private readonly IReminderService _reminderService;

  public ReminderController(IReminderService reminderService)
  {
    _reminderService = reminderService;
  }

  [Topic("reminderpubsub", "newreminder")]
  [HttpPost("newreminder")]
  public async Task<IActionResult> HandleNewReminder([FromBody] TaskItem taskItem)
  {
    await _reminderService.HandleNewReminderAsync(taskItem);
    return Ok();
  }

  [Topic("reminderpubsub", "updatereminder")]
  [HttpPost("updatereminder")]
  public async Task<IActionResult> HandleUpdateReminder([FromBody] TaskItem taskItem)
  {
    await _reminderService.HandleUpdateReminderAsync(taskItem);
    return Ok();
  }

  [Topic("reminderpubsub", "deletereminder")]
  [HttpPost("deletereminder")]
  public async Task<IActionResult> HandleDeleteReminder([FromBody] TaskItem taskItem)
  {
    await _reminderService.HandleDeleteReminderAsync(taskItem);
    return Ok();
  }
}