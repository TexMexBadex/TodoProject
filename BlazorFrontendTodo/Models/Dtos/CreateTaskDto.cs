namespace BlazorFrontendTodo.Models.Dtos
{
    public class CreateTaskDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? Reminder { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
