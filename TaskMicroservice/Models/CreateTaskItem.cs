namespace TaskMicroservice.Models
{
  public class CreateTaskItem
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } //Hver task er knyttet til en bruger
    public string Content { get; set; }
    public DateTime? Reminder { get; set; }
    public bool IsCompleted { get; set; } = false;
  }
}
