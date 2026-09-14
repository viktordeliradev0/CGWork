using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CGWork.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string SchoolClass { get; set; } = string.Empty; // Напр. "5-ти клас" или "10А"

        // 4 Месечни вноски с техните дати на плащане
        public bool PaidMonth1 { get; set; } = false;
        public DateTime? PaidMonth1Date { get; set; }

        public bool PaidMonth2 { get; set; } = false;
        public DateTime? PaidMonth2Date { get; set; }

        public bool PaidMonth3 { get; set; } = false;
        public DateTime? PaidMonth3Date { get; set; }

        public bool PaidMonth4 { get; set; } = false;
        public DateTime? PaidMonth4Date { get; set; }

        // Информация за контакт с родител
        public string ParentName { get; set; } = string.Empty;
        public string ParentPhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Невалиден формат на имейла.")]
        public string? ParentEmail { get; set; }

        // Връзка към групата
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        // Връзка към присъствията
        public ICollection<LessonSession> AttendedLessons { get; set; } = new List<LessonSession>();

        // Автоматично изчисляване на броя присъствия
       

       
    }
}