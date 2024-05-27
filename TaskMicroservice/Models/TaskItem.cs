namespace TaskMicroservice.Models;

public class TaskItem
{
  public Guid Id { get; set; }
  public string UserId { get; set; } = string.Empty; //Hver task er knyttet til en bruger
  public string UserEmail { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public DateTime? Reminder { get; set; }
  public bool IsCompleted { get; set; }
}