using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitConnectAPI.Data;
using HabitConnectAPI.Entities;

namespace HabitConnectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalsController : ControllerBase
    {
        private readonly HabitConnectContext _context;
        public JournalsController(HabitConnectContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Journal>>> GetJournals() => await _context.Journals.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Journal>> GetJournal(int id)
        {
            var journal = await _context.Journals.FindAsync(id);
            if (journal == null) return NotFound();
            return journal;
        }

        [HttpPost]
        public async Task<ActionResult<Journal>> PostJournal(Journal journal)
        {
            // ensure date is present
            journal.Date = journal.Date == default ? DateTime.UtcNow.Date : journal.Date.Date;
            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJournal), new { id = journal.Id }, journal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutJournal(int id, Journal journal)
        {
            if (id != journal.Id) return BadRequest();
            _context.Entry(journal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJournal(int id)
        {
            var journal = await _context.Journals.FindAsync(id);
            if (journal == null) return NotFound();
            _context.Journals.Remove(journal);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // פעולה נוספת: שליפת כל היומנים של הרגל מסוים
        [HttpGet("byHabit/{habitId}")]
        public async Task<ActionResult<IEnumerable<Journal>>> GetJournalsByHabit(int habitId)
        {
            var journals = await _context.Journals.Where(j => j.HabitId == habitId).OrderByDescending(j => j.Date).ToListAsync();
            return journals;
        }
    }
}
