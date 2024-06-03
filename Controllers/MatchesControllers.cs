using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltraPlayBettingData.Data;



namespace UltraPlayBettingData.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MatchesControllers : ControllerBase
    {
        private readonly BettingContext contextValue;

        public MatchesControllers(BettingContext context)
        {
            contextValue = context;
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMatches()
        {
            var next24Hours = DateTime.UtcNow.AddHours(24);
            var matches = await contextValue.Matches
                .Where(m => m.StartDate >= DateTime.UtcNow && m.StartDate <= next24Hours)
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .ToListAsync();

            var result = matches.Select(m => new
            {
                m.Name,
                m.StartDate,
                Bets = m.Bets.Where(b => b.Name == "Match Winner" || b.Name == "Map Advantage" || b.Name == "Total Maps Played")
                             .Select(b => new
                             {
                                 b.Name,
                                 Odds = b.Odds.GroupBy(o => o.SpecialBetValue)
                                              .Select(g => new
                                              {
                                                  SpecialBetValue = g.Key,
                                                  Odds = g.ToList()
                                              }).ToList()
                             }).ToList()
            });

            return Ok(result);
        }
    }

}
