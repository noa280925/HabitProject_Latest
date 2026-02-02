using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitConnectAPI.Data;
using HabitConnectAPI.Entities;

namespace HabitConnectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitsController : ControllerBase
    {
        private readonly HabitConnectContext _context;
        public HabitsController(HabitConnectContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Habit>>> GetHabits() => await _context.Habits.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Habit>> GetHabit(int id)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();
            return habit;
        }

        [HttpPost]
        public async Task<ActionResult<Habit>> PostHabit(Habit habit)
        {
            habit.CreatedDate = habit.CreatedDate == default ? DateTime.UtcNow : habit.CreatedDate;
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHabit(int id, Habit habit)
        {
            if (id != habit.Id) return BadRequest();
            _context.Entry(habit).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabit(int id)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();
            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // פעולה נוספת לפי הדרישות: שינוי סטטוס הפעילות של ההרגל (פעיל/לא פעיל)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool isActive)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();
            habit.IsActive = isActive;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
