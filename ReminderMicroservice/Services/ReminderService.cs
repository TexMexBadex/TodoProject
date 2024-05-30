using Dapr.Actors;
using Dapr.Actors.Client;
using ReminderMicroservice.Actors;
using ReminderMicroservice.Actors.Interfaces;
using ReminderMicroservice.Models;
using ReminderMicroservice.Services.Interfaces;
//  public class ReminderService : IReminderService
//  {
//    private readonly ILogger<ReminderService> _logger;
//    public ReminderService(ILogger<ReminderService> logger)
//    {
//      _logger = logger;
//    }

//    public async Task HandleNewReminderAsync(ReminderModel reminder)
//    {

//      _logger.LogInformation($"New reminder received for task: {reminder.Id}");
//      var actorId = new ActorId(reminder.Id.ToString());
//      var actor = ActorProxy.Create<IReminderActor>(actorId, nameof(ReminderActor));
//      try
//      {
//        await actor.RegisterTimerAsync(reminder);
//      }
//      catch (Exception ex)
//      {
//        _logger.LogError(ex, $"Failed to set reminder for task: {reminder.Id}");
//      }
//    }

//    public async Task HandleUpdateReminderAsync(ReminderModel reminder)
//    {
//      _logger.LogInformation($"Update reminder received for task: {reminder.Id}");
//      var actorId = new ActorId(reminder.Id.ToString());
//      var actor = ActorProxy.Create<IReminderActor>(actorId, nameof(ReminderActor));
//      await actor.RegisterTimerAsync(reminder);
//    }

//    public async Task HandleDeleteReminderAsync(TaskItem taskItem)
//    {
//      _logger.LogInformation($"Delete reminder received for task: {taskItem.Id}");
//      var actorId = new ActorId(taskItem.Id.ToString());
//      var actor = ActorProxy.Create<IReminderActor>(actorId, nameof(ReminderActor));
//      await actor.DeleteReminder(taskItem.Id.ToString());
//    }
//  }
//}


namespace ReminderMicroservice.Services
{
  namespace ReminderMicroservice.Services
  {
    public class ReminderService : IReminderService
    {
      private readonly ILogger<ReminderService> _logger;

      public ReminderService(ILogger<ReminderService> logger)
      {
        _logger = logger;
      }

      public async Task HandleNewReminderAsync(ReminderModel reminderModel)
      {
        _logger.LogInformation($"New reminder received for task: {reminderModel.Id}");
        var actorId = new ActorId(reminderModel.Id.ToString());
        var actor = ActorProxy.Create<IReminderActor>(actorId, "ReminderActor");
        try
        {
          await actor.RegisterTimerAsync(reminderModel);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Failed to set reminder for task: {reminderModel.Id}");
        }
      }

      public async Task HandleUpdateReminderAsync(ReminderModel reminderModel)
      {
        _logger.LogInformation($"Update reminder received for task: {reminderModel.Id}");
        var actorId = new ActorId(reminderModel.Id.ToString());
        var actor = ActorProxy.Create<IReminderActor>(actorId, "ReminderActor");
        await actor.RegisterTimerAsync(reminderModel);
      }

      public async Task HandleDeleteReminderAsync(TaskItem taskItem)
      {
        _logger.LogInformation($"Delete reminder received for task: {taskItem.Id}");
        var actorId = new ActorId(taskItem.Id.ToString());
        var actor = ActorProxy.Create<IReminderActor>(actorId, "ReminderActor");
        await actor.DeleteReminder(taskItem.Id.ToString());
      }
    }
  }
}

