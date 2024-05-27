using System.Runtime.Serialization;
namespace ReminderMicroservice.Models
{
[DataContract]

  public class TaskItem
  {
  [DataMember]
    public Guid Id { get; set; }

    [DataMember]

    public string UserEmail { get; set; } = string.Empty;
    [DataMember]


    public string Content { get; set; } = string.Empty;
    [DataMember]

    public DateTime? Reminder { get; set; }

  }
}
