using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using ReminderMicroservice.Models;
using ReminderMicroservice.Services.Interfaces;
namespace ReminderMicroservice.Controllers
{
  [ApiController]
  public class ReminderController : ControllerBase
  {
    private readonly ILogger<ReminderController> _logger;
    private readonly IReminderService _reminderService;

    public const string PubSub = "pubsub";

    public ReminderController(ILogger<ReminderController> logger, IReminderService reminderService)
    {
      _logger = logger;
      _reminderService = reminderService;
    }

    [Topic(PubSub, "newreminder")]
    [HttpPost("newreminder")]
    public async Task<IActionResult> HandleNewReminder(TaskItem taskItem)
    {
      var reminder = new ReminderModel
      {
        Id = taskItem.Id,
        Content = taskItem.Content,
        Reminder = taskItem.Reminder.Value,
        UserEmail = taskItem.UserEmail
      };

      _logger.LogInformation("Received new reminder event");
      await _reminderService.HandleNewReminderAsync(reminder);
      return Ok();
    }

    [Topic(PubSub, "updatereminder")]
    [HttpPost("updatereminder")]
    public async Task<IActionResult> HandleUpdateReminder([FromBody] TaskItem taskItem)
    {
      var reminder = new ReminderModel
      {
        Id = taskItem.Id,
        Content = taskItem.Content,
        Reminder = taskItem.Reminder.Value,
        UserEmail = taskItem.UserEmail
      };

      _logger.LogInformation("Received update reminder event");
      await _reminderService.HandleUpdateReminderAsync(reminder);
      return Ok();
    }

    [Topic(PubSub, "deletereminder")]
    [HttpPost("deletereminder")]
    public async Task<IActionResult> HandleDeleteReminder([FromBody] TaskItem taskItem)
    {
      _logger.LogInformation("Received delete reminder event");
      await _reminderService.HandleDeleteReminderAsync(taskItem);
      return Ok();
    }
  }
}