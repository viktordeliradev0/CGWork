using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace CGWork.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string City { get; set; } = "Пловдив";

        // Връзка към групите, ако този потребител е Учител
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}