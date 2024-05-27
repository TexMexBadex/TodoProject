namespace TaskMicroservice.Models
{
  public class EditTaskItem
  {
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? Reminder { get; set; }
    public bool IsCompleted { get; set; }
  }
}
