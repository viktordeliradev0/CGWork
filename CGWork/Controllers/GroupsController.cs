using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CGWork.Data;
using CGWork.Models;
using System.Security.Claims;

namespace CGWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Всички заявки изискват вход
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Помощен метод за проверка дали логнатият потребител е мениджър
        private bool IsUserAdmin()
        {
            // 1. Проверка по роля
            if (User.IsInRole("Manager") || User.IsInRole("Мениджър"))
                return true;

            // 2. Вземаме името/имейла от Claims
            var userEmail = User.FindFirstValue(ClaimTypes.Email)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.Identity?.Name;

            // ТУК: Сложи твоя точно имейл на Мениджъра (напр. manager@codinggiants.bg или с който влизаш)
            if (!string.IsNullOrEmpty(userEmail) && userEmail.Equals("viktordeliradev0@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return await _context.Groups.ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return group;
        }

        // POST: api/Groups (Създаване на група)
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Само мениджъри могат да създават нови групи!");
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        // PUT: api/Groups/5 (Редактиране на група)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group group)
        {
            if (id != group.Id) return BadRequest();

            var existingGroup = await _context.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
            if (existingGroup == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Разрешаваме редакция, ако е Мениджър ИЛИ ако е учителят на тази група
            bool hasAccess = IsUserAdmin() ||
                             (!string.IsNullOrEmpty(existingGroup.TeacherId) && existingGroup.TeacherId.Equals(userId, StringComparison.OrdinalIgnoreCase));

            if (!hasAccess)
            {
                return Forbid("Нямате права да редактирате чужда група!");
            }

            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Groups/5 (Изтриване на група)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            if (!IsUserAdmin())
            {
                return Forbid("Само мениджърът има право да изтрива групи!");
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound("Групата не е намерена.");

            // 1. Намираме и премахваме свързаните уроци
            var lessons = await _context.LessonSessions.Where(l => l.GroupId == id).ToListAsync();
            _context.LessonSessions.RemoveRange(lessons);

            // 2. Намираме учениците и или ги изтриваме, или им махаме GroupId
            var students = await _context.Students.Where(s => s.GroupId == id).ToListAsync();
            _context.Students.RemoveRange(students); // Или s.GroupId = null, ако искаш да ги пазиш

            // 3. Сега вече изтриваме самата група чисто
            _context.Groups.Remove(group);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}