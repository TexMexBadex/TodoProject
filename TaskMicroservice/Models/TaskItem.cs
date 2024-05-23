public class TaskItem
{
  public Guid Id { get; set; } 
  public string UserId { get; set; } //Hver task er knyttet til en bruger
  public string UserEmail { get; set; }
  public string Content { get; set; }
  public DateTime? Reminder { get; set; }
  public bool IsCompleted { get; set; }
}
