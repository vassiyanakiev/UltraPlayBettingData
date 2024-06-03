using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltraPlayBettingData.Data;
using UltraPlayBettingData.DTO_s;


namespace UltraPlayBettingData.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MatchesControllers : ControllerBase
    {
        private readonly BettingContext _dbContext;
        private readonly IMapper _mapper;

        public MatchesControllers(BettingContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Endpoint 1: Get all matches starting in the next 24 hours
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesStartingInNext24Hours()
        {
            var matches = await _dbContext.Matches
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .Where(m => m.StartDate >= DateTime.UtcNow && m.StartDate <= DateTime.UtcNow.AddHours(24))
                .ToListAsync();

            var matchDtos = _mapper.Map<List<MatchDto>>(matches);

            foreach (var matchDto in matchDtos)
            {
                matchDto.ActivePreviewBets = matchDto.ActivePreviewBets
                    .Where(b => b.Name == "Match Winner" || b.Name == "Map Advantage" || b.Name == "Total Maps Played")
                    .Select(b =>
                    {
                        if (b.Odds.Any(o => !string.IsNullOrEmpty(o.SpecialBetValue)))
                        {
                            b.Odds = b.Odds
                                .GroupBy(o => o.SpecialBetValue)
                                .Select(g => g.First())
                                .ToList();
                        }
                        return b;
                    })
                    .ToList();
            }

            return Ok(matchDtos);
        }

        // Endpoint 2: Get a single match by its unique identifier
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatchById(int id)
        {
            var match = await _dbContext.Matches
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }

            var matchDto = _mapper.Map<MatchDto>(match);

            return Ok(matchDto);
        }
    }

}
