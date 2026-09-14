using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CGWork.Data;
using CGWork.Models;

namespace CGWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students
                .Include(s => s.AttendedLessons)
                .ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.AttendedLessons)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();
            return student;
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            ValidateFeeDates(student);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // PUT: api/Students/5
        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id) return BadRequest();

            var existingStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (existingStudent == null) return NotFound();

            // Ако потребителят НЕ Е мениджър, запазваме старите стойности за плащанията
            if (!User.IsInRole("Manager"))
            {
                student.PaidMonth1 = existingStudent.PaidMonth1;
                student.PaidMonth1Date = existingStudent.PaidMonth1Date;
                student.PaidMonth2 = existingStudent.PaidMonth2;
                student.PaidMonth2Date = existingStudent.PaidMonth2Date;
                student.PaidMonth3 = existingStudent.PaidMonth3;
                student.PaidMonth3Date = existingStudent.PaidMonth3Date;
                student.PaidMonth4 = existingStudent.PaidMonth4;
                student.PaidMonth4Date = existingStudent.PaidMonth4Date;
            }
            else
            {
                ValidateFeeDates(student);
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        // Автоматично изчиства датата, ако чекбоксът за дадена месечна такса е премахнат
        private static void ValidateFeeDates(Student student)
        {
            if (!student.PaidMonth1) student.PaidMonth1Date = null;
            if (!student.PaidMonth2) student.PaidMonth2Date = null;
            if (!student.PaidMonth3) student.PaidMonth3Date = null;
            if (!student.PaidMonth4) student.PaidMonth4Date = null;
        }
    }
}