using System.Text.Json.Serialization;

namespace CGWork.Models
{
    public class LessonSession
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Topic { get; set; } = string.Empty; // Какво е предавано

        // За коя група е този урок
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public string LessonType { get; set; } = "Редовен";

        public string? TeacherId { get; set; }

        // Списък с децата, които СА присъствали на този конкретен урок
        
        [JsonIgnore]
        public ICollection<Student> AttendingStudents { get; set; } = new List<Student>();
    }
}
