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
    public class SemestersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SemestersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Semester>>> GetSemesters()
        {
            return await _context.Semesters.OrderByDescending(s => s.StartDate).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Semester>> PostSemester(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSemesters), new { id = semester.Id }, semester);
        }
    }
}