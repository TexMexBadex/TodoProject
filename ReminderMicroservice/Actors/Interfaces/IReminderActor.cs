using Dapr.Actors;
using ReminderMicroservice.Models;

namespace ReminderMicroservice.Actors.Interfaces
{
    public interface IReminderActor : IActor
    {
        Task RegisterTimerAsync(ReminderModel reminderModel);
       
        Task DeleteReminder(string reminderName);

       
    }
}
