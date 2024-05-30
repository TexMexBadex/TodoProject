using System.Runtime.Serialization;

namespace ReminderMicroservice.Models
{

  public class ReminderModel
  {

    public Guid Id { get; set; }


    public string UserEmail { get; set; } = string.Empty;


    public string Content { get; set; } = string.Empty;


    public DateTime Reminder { get; set; }
  }
}