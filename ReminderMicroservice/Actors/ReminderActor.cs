using Dapr.Actors.Runtime;
using ReminderMicroservice.Actors.Interfaces;
using ReminderMicroservice.Models;
using ReminderMicroservice.Services;
using ReminderMicroservice.Services.Interfaces;

namespace ReminderMicroservice.Actors
{
  public class ReminderActor : Actor, IReminderActor, IRemindable
  {

    private readonly IEmailService _emailService;
    private readonly ILogger<ReminderActor> _logger;


    public ReminderActor(ActorHost host, IEmailService emailService, ILogger<ReminderActor> logger) : base(host)
    {
      _emailService = emailService;
      _logger = logger;
    }

    // Set a new reminder and also update a current one
    public async Task RegisterTimerAsync(ReminderModel taskItem)
    {
      await StateManager.SetStateAsync("reminder", taskItem);
      _logger.LogInformation("State set. Now trying to register reminder.");

      var dueTime = taskItem.Reminder - DateTime.UtcNow;
      _logger.LogInformation($"DueTime set to: {dueTime.Minutes} minutes from now.");

      await RegisterReminderAsync(taskItem.Id.ToString(), null, dueTime, TimeSpan.FromMilliseconds(-1));
      _logger.LogInformation($"Reminder registered for task {taskItem.Id}.");
    }

    // Delete a reminder
    public async Task DeleteReminder(string reminderName)
    {
      _logger.LogInformation("Deletion of reminder received. Trying to delete reminder.");
      await UnregisterReminderAsync(reminderName);

      _logger.LogInformation("Removing state for reminder.");
      await StateManager.RemoveStateAsync("reminder");

      _logger.LogInformation("State removed.");

    }

    // Trigger the reminder
    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
      _logger.LogInformation("ReceiveReminderAsync triggered...");
      var task = await StateManager.GetStateAsync<TaskItem>("reminder");
      if (reminderName == task.Id.ToString())
      {
        await _emailService.SendEmailAsync(
          task.UserEmail,
          "Task Reminder",
          $"Reminder for task: {task.Content} at {task.Reminder}");
      }
    }
  }
}


