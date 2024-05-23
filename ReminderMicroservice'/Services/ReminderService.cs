using Dapr.Actors;
using Dapr.Actors.Client;
using ReminderMicroservice_.Actors;
using ReminderMicroservice_.Actors.Interfaces;
using ReminderMicroservice_.Models;
using ReminderMicroservice_.Services.Interfaces;

namespace ReminderMicroservice.Services
{
  public class ReminderService : IReminderService
  {
    private readonly ILogger<ReminderService> _logger;
    private readonly IActorProxyFactory _actorProxyFactory;
    public ReminderService(ILogger<ReminderService> logger, IActorProxyFactory actorProxyFactory)
    {
      _logger = logger;
      _actorProxyFactory = actorProxyFactory;
    }

    public async Task HandleNewReminderAsync(TaskItem taskItem)
    {
      _logger.LogInformation($"New reminder received for task: {taskItem.Id}");
      var actorId = new ActorId(taskItem.Id.ToString());
      var actor = _actorProxyFactory.CreateActorProxy<IReminderActor>(actorId, nameof(ReminderActor));
      await actor.SetReminder(taskItem);
    }

    public async Task HandleUpdateReminderAsync(TaskItem taskItem)
    {
      _logger.LogInformation($"Update reminder received for task: {taskItem.Id}");
      var actorId = new ActorId(taskItem.Id.ToString());
      var actor = _actorProxyFactory.CreateActorProxy<IReminderActor>(actorId, nameof(ReminderActor));
      await actor.UpdateReminder(taskItem);
    }

    public async Task HandleDeleteReminderAsync(TaskItem taskItem)
    {
      _logger.LogInformation($"Delete reminder received for task: {taskItem.Id}");
      var actorId = new ActorId(taskItem.Id.ToString());
      var actor = _actorProxyFactory.CreateActorProxy<IReminderActor>(actorId, nameof(ReminderActor));
      await actor.DeleteReminder(taskItem.Id.ToString());
    }
  }
}