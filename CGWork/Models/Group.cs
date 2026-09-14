namespace CGWork.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // --- НОВИ ПОЛЕТА ЗА СЕМЕСТЪР ---
        public int? SemesterId { get; set; }
        public Semester? Semester { get; set; }

        public string City { get; set; } = "Пловдив";
        public DateTime StartDate { get; set; } = DateTime.Today; // Кога започва първият урок
        public int DurationInMonths { get; set; } = 4;

        // Връзка към Учителя
        public string TeacherId { get; set; } = string.Empty;

        
        public ApplicationUser? Teacher { get; set; }

        // Децата в тази група
        public ICollection<Student> Students { get; set; } = new List<Student>();

        // Проведените уроци за тази група
        public ICollection<LessonSession> Lessons { get; set; } = new List<LessonSession>();
    }
}