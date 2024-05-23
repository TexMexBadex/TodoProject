using Dapr.Actors;
using ReminderMicroservice_.Models;

namespace ReminderMicroservice_.Actors.Interfaces
{
    public interface IReminderActor : IActor
    {
        Task SetReminder(TaskItem taskItem);
        Task UpdateReminder(TaskItem taskItem);
        Task DeleteReminder(string reminderName);

       
    }
}
