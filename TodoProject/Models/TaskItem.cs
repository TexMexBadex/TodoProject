namespace TodoProject.Models
{
  public class TaskItem
  {
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? ScheduledFor { get; set; }
  }

}
