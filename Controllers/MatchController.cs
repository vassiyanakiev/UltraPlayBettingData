using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltraPlayBettingData.Data;

namespace UltraPlayBettingData.Controllers
{
    public class MatchController : ControllerBase
    {
        private readonly BettingContext contextValue;

        public MatchController(BettingContext context)
        {
            contextValue = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(int id)
        {
            var match = await contextValue.Matches
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (match == null)
            {
                return NotFound();
            }

            var result = new
            {
                match.Name,
                match.StartDate,
                ActiveBets = match.Bets.Where(b => b.IsLive).Select(b => new
                {
                    b.Name,
                    b.IsLive,
                    Odds = b.Odds
                }),
                InactiveBets = match.Bets.Where(b => !b.IsLive).Select(b => new
                {
                    b.Name,
                    b.IsLive,
                    Odds = b.Odds
                })
            };

            return Ok(result);
        }
    }
}
