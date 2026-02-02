using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitConnectAPI.Data;
using HabitConnectAPI.Entities;

namespace HabitConnectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserCommunitiesController : ControllerBase
    {
        private readonly HabitConnectContext _context;
        public UserCommunitiesController(HabitConnectContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCommunity>>> GetAll() => await _context.UserCommunities.ToListAsync();

        [HttpGet("byCommunity/{communityId}")]
        public async Task<ActionResult<IEnumerable<UserCommunity>>> GetByCommunity(int communityId)
        {
            var list = await _context.UserCommunities.Where(uc => uc.CommunityId == communityId).ToListAsync();
            return list;
        }

        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<IEnumerable<UserCommunity>>> GetByUser(int userId)
        {
            var list = await _context.UserCommunities.Where(uc => uc.UserId == userId).ToListAsync();
            return list;
        }

        [HttpPost]
        public async Task<ActionResult<UserCommunity>> Join(UserCommunity uc)
        {
            // optional: prevent duplicates
            var exists = await _context.UserCommunities.AnyAsync(x => x.UserId == uc.UserId && x.CommunityId == uc.CommunityId);
            if (exists) return Conflict(new { message = "User already in community" });

            _context.UserCommunities.Add(uc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = uc.Id }, uc);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Leave(int id)
        {
            var uc = await _context.UserCommunities.FindAsync(id);
            if (uc == null) return NotFound();
            _context.UserCommunities.Remove(uc);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
