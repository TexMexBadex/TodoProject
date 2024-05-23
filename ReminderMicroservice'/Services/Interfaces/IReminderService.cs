using ReminderMicroservice_.Models;

namespace ReminderMicroservice_.Services.Interfaces
{
    public interface IReminderService
    {
      Task HandleNewReminderAsync(TaskItem taskItem);
      Task HandleUpdateReminderAsync(TaskItem taskItem);
      Task HandleDeleteReminderAsync(TaskItem taskItem);
  }
}
