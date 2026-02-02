using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitConnectAPI.Data;
using HabitConnectAPI.Entities;

namespace HabitConnectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunitiesController : ControllerBase
    {
        private readonly HabitConnectContext _context;
        public CommunitiesController(HabitConnectContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Community>>> GetCommunities() => await _context.Communities.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Community>> GetCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null) return NotFound();
            return community;
        }

        [HttpPost]
        public async Task<ActionResult<Community>> PostCommunity(Community community)
        {
            community.CreatedDate = DateTime.UtcNow;
            _context.Communities.Add(community);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCommunity), new { id = community.Id }, community);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommunity(int id, Community community)
        {
            if (id != community.Id) return BadRequest();
            _context.Entry(community).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null) return NotFound();
            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // פעולה נוספת: חישוב ImpactScore לקהילה (30 ימים אחרונים)
        [HttpGet("{id}/impact")]
        public async Task<IActionResult> GetImpactScore(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null) return NotFound();

            // get members
            var userIds = await _context.UserCommunities
                .Where(uc => uc.CommunityId == id)
                .Select(uc => uc.UserId)
                .ToListAsync();

            if (!userIds.Any()) return Ok(new { communityId = id, impactScore = 0, message = "No members" });

            var since = DateTime.UtcNow.AddDays(-30);

            // journals for habits belonging to these users
            var journals = await (from j in _context.Journals
                                  join h in _context.Habits on j.HabitId equals h.Id
                                  where userIds.Contains(h.UserId) && j.Date >= since
                                  select j).ToListAsync();

            if (!journals.Any()) return Ok(new { communityId = id, impactScore = 0, message = "No recent journal entries" });

            var successCount = journals.Count(j => j.Success);
            var total = journals.Count;

            var score = (int)Math.Round((double)successCount / total * 100);

            return Ok(new { communityId = id, impactScore = score, success = successCount, total = total });
        }
    }
}
