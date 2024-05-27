using ReminderMicroservice.Models;

namespace ReminderMicroservice.Services.Interfaces
{
    public interface IReminderService
    {
      Task HandleNewReminderAsync(ReminderModel reminder);
      Task HandleUpdateReminderAsync(ReminderModel reminder);
      Task HandleDeleteReminderAsync(TaskItem reminderModel);
  }
}
