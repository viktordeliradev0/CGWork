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
    [Authorize]
    public class LessonSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Помощен метод за проверка дали текущият потребител е мениджър
        private bool IsUserAdmin()
        {
            return User.IsInRole("Manager") || User.IsInRole("Мениджър");
        }

        // ==========================================
        // 1. ВЗИМАНЕ НА ВСИЧКИ УРОЦИ (GET)
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonSession>>> GetLessonSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Ако е Мениджър, връщаме всичко
            if (IsUserAdmin() || string.IsNullOrEmpty(userId))
            {
                return await _context.LessonSessions
                    .Include(l => l.AttendingStudents)
                    .OrderByDescending(l => l.Date)
                    .ToListAsync();
            }

            // Ако е Учител, връщаме само неговите уроци
            return await _context.LessonSessions
                .Include(l => l.AttendingStudents)
                .Where(l => l.TeacherId == userId)
                .OrderByDescending(l => l.Date)
                .ToListAsync();
        }

        // ==========================================
        // 2. СЪЗДАВАНЕ НА НОВ УРОК (POST)
        // ==========================================
        [HttpPost]
        public async Task<ActionResult<LessonSession>> PostLessonSession(LessonSession lesson)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var group = await _context.Groups.FindAsync(lesson.GroupId);

            // ПРОВЕРКА ЗА ПРАВА: Мениджър, Автор на урока или Титуляр на групата
            bool hasAccess = IsUserAdmin() ||
                             lesson.TeacherId == userId ||
                             (group != null && group.TeacherId == userId);

            if (!hasAccess)
            {
                return Forbid("Нямате права да създавате урок за тази група!");
            }

            _context.LessonSessions.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonSessions), new { id = lesson.Id }, lesson);
        }

        // ==========================================
        // 3. ОТБЕЛЯЗВАНЕ И РЕДАКТИРАНЕ НА ПРИСЪСТВИЯ (POST)
        // ==========================================
        [HttpPost("{id}/attendance")]
        public async Task<IActionResult> MarkAttendance(int id, [FromBody] List<int> studentIds)
        {
            var lesson = await _context.LessonSessions
                .Include(l => l.AttendingStudents)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound("Урокът не е намерен.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var group = await _context.Groups.FindAsync(lesson.GroupId);

            // ПРОВЕРКА ЗА ПРАВА: Мениджър, Автор на урока или Титуляр на групата
            bool hasAccess = IsUserAdmin() ||
                             lesson.TeacherId == userId ||
                             (group != null && group.TeacherId == userId);

            if (!hasAccess)
            {
                return Forbid("Нямате права да отбелязвате присъствия за този урок!");
            }

            // Изчистваме старите присъствия
            lesson.AttendingStudents.Clear();

            // Добавяме новите присъствия
            if (studentIds != null && studentIds.Any())
            {
                var students = await _context.Students
                    .Where(s => studentIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var student in students)
                {
                    lesson.AttendingStudents.Add(student);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Присъствията са успешно обновени!" });
        }

        // ==========================================
        // 4. РЕДАКТИРАНЕ НА САМИЯ УРОК (PUT)
        // ==========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLessonSession(int id, LessonSession lessonSession)
        {
            if (id != lessonSession.Id) return BadRequest("Невалидно ID на урока.");

            var existingLesson = await _context.LessonSessions.FindAsync(id);
            if (existingLesson == null) return NotFound("Урокът не е намерен.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var group = await _context.Groups.FindAsync(existingLesson.GroupId);

            // ПРОВЕРКА ЗА ПРАВА: Мениджър, Автор на урока или Титуляр на групата
            bool hasAccess = IsUserAdmin() ||
                             existingLesson.TeacherId == userId ||
                             (group != null && group.TeacherId == userId);

            if (!hasAccess)
            {
                return Forbid("Нямате права да редактирате този урок!");
            }

            // Обновяваме данните
            existingLesson.Date = lessonSession.Date;
            existingLesson.Topic = lessonSession.Topic;
            existingLesson.LessonType = lessonSession.LessonType;
            existingLesson.TeacherId = lessonSession.TeacherId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // 5. ИЗТРИВАНЕ НА УРОК (DELETE)
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessonSession(int id)
        {
            var lessonSession = await _context.LessonSessions.FindAsync(id);
            if (lessonSession == null) return NotFound("Урокът не е намерен.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var group = await _context.Groups.FindAsync(lessonSession.GroupId);

            // ПРОВЕРКА ЗА ПРАВА: Мениджър, Автор на урока или Титуляр на групата
            bool hasAccess = IsUserAdmin() ||
                             lessonSession.TeacherId == userId ||
                             (group != null && group.TeacherId == userId);

            if (!hasAccess)
            {
                return Forbid("Нямате права да изтриете този урок!");
            }

            _context.LessonSessions.Remove(lessonSession);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}