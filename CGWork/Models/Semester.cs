namespace CGWork.Models
{
    public class Semester
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Напр. "Есен 2026"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Списък с групите, които се провеждат през този семестър
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}