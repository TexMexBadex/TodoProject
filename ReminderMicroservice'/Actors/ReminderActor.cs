using Dapr.Actors.Runtime;
using ReminderMicroservice_.Actors.Interfaces;
using ReminderMicroservice_.Models;
using ReminderMicroservice_.Services.Interfaces;

namespace ReminderMicroservice_.Actors
{
  public class ReminderActor : Actor, IReminderActor
  {
    private readonly IEmailService _emailService;

    public ReminderActor(ActorHost host, IEmailService emailService) : base(host)
    {
      _emailService = emailService;
    }

    // Set a new reminder
    public async Task SetReminder(TaskItem taskItem)
    {
      await StateManager.SetStateAsync("task", taskItem);
      await RegisterReminderAsync(taskItem.Id.ToString(), null, taskItem.Reminder.Value - DateTime.UtcNow, TimeSpan.FromMilliseconds(-1));
    }

    // Update an existing reminder
    public async Task UpdateReminder(TaskItem taskItem)
    {
      await StateManager.SetStateAsync("task", taskItem);
      await UnregisterReminderAsync(taskItem.Id.ToString());
      await RegisterReminderAsync(taskItem.Id.ToString(), null, taskItem.Reminder.Value - DateTime.UtcNow, TimeSpan.FromMilliseconds(-1));
    }

    // Delete a reminder
    public async Task DeleteReminder(string reminderName)
    {
      await UnregisterReminderAsync(reminderName);
      await StateManager.RemoveStateAsync("task");
    }

    // Trigger the reminder
    public async Task ReceiveReminderAsync(string reminderName, byte[] state)
    {
      var taskItem = await StateManager.GetStateAsync<TaskItem>("task");
      await _emailService.SendEmailAsync(taskItem.UserEmail, "Task Reminder", $"Reminder for task: {taskItem.Content} at {taskItem.Reminder}");
    }
  }
}