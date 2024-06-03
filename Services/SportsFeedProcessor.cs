using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using UltraPlayBettingData.Data;
using UltraPlayBettingData.Models;

namespace UltraPlayBettingData.Services
{
    public class SportsFeedProcessor
    {
        private static readonly HttpClient client = new HttpClient();
        private const string FeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";
        private readonly IServiceScopeFactory scopeFactoryValue;
        private readonly UpdateService updateServiceValue;

        public SportsFeedProcessor(IServiceScopeFactory scopeFactory, UpdateService updateService)
        {
            scopeFactoryValue = scopeFactory;
            updateServiceValue = updateService;
        }

        public async Task FetchAndSaveFeed()
        {
            try
            {
                var response = await client.GetStringAsync(FeedUrl);
                var xmlSports = DeserializeFeed(response);

                using (var scope = scopeFactoryValue.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<BettingContext>();

                    // Upsert Sport
                    var existingSport = await context.Sports.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.ID == xmlSports.Sport.ID);

                    if (existingSport != null)
                    {
                        UpdateSport(existingSport, xmlSports.Sport, context);
                    }
                    else
                    {
                        context.Sports.Add(xmlSports.Sport);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateSport(Sport existingSport, Sport newSport, BettingContext context)
        {
            context.Entry(existingSport).CurrentValues.SetValues(newSport);

            foreach (var newEvent in newSport.Events)
            {
                var existingEvent = context.Events.AsNoTracking()
                    .FirstOrDefault(e => e.ID == newEvent.ID);

                if (existingEvent != null)
                {
                    UpdateEvent(existingEvent, newEvent, context);
                }
                else
                {
                    context.Events.Add(newEvent);
                }
            }

            // Remove deleted events
            var existingEvents = context.Events.Where(e => e.SportID == newSport.ID).ToList();
            foreach (var existingEvent in existingEvents)
            {
                if (!newSport.Events.Any(e => e.ID == existingEvent.ID))
                {
                    context.Events.Remove(existingEvent);
                }
            }
        }

        private void UpdateEvent(Event existingEvent, Event newEvent, BettingContext context)
        {
            context.Entry(existingEvent).CurrentValues.SetValues(newEvent);

            foreach (var newMatch in newEvent.Matches)
            {
                var existingMatch = context.Matches.AsNoTracking()
                    .FirstOrDefault(m => m.ID == newMatch.ID);

                if (existingMatch != null)
                {
                    UpdateMatch(existingMatch, newMatch, context);
                }
                else
                {
                    context.Matches.Add(newMatch);
                }
            }

            // Remove deleted matches
            var existingMatches = context.Matches.Where(m => m.EventID == newEvent.ID).ToList();
            foreach (var existingMatch in existingMatches)
            {
                if (!newEvent.Matches.Any(m => m.ID == existingMatch.ID))
                {
                    context.Matches.Remove(existingMatch);
                }
            }
        }

        private void UpdateMatch(Match existingMatch, Match newMatch, BettingContext context)
        {
            context.Entry(existingMatch).CurrentValues.SetValues(newMatch);

            foreach (var newBet in newMatch.Bets)
            {
                var existingBet = context.Bets.AsNoTracking()
                    .FirstOrDefault(b => b.ID == newBet.ID);

                if (existingBet != null)
                {
                    UpdateBet(existingBet, newBet, context);
                }
                else
                {
                    context.Bets.Add(newBet);
                }
            }

            // Remove deleted bets
            var existingBets = context.Bets.Where(b => b.MatchID == newMatch.ID).ToList();
            foreach (var existingBet in existingBets)
            {
                if (!newMatch.Bets.Any(b => b.ID == existingBet.ID))
                {
                    context.Bets.Remove(existingBet);
                }
            }
        }

        private void UpdateBet(Bet existingBet, Bet newBet, BettingContext context)
        {
            context.Entry(existingBet).CurrentValues.SetValues(newBet);

            foreach (var newOdd in newBet.Odds)
            {
                var existingOdd = context.Odds.AsNoTracking()
                    .FirstOrDefault(o => o.ID == newOdd.ID);

                if (existingOdd != null)
                {
                    context.Entry(existingOdd).CurrentValues.SetValues(newOdd);
                }
                else
                {
                    context.Odds.Add(newOdd);
                }
            }

            // Remove deleted odds
            var existingOdds = context.Odds.Where(o => o.BetID == newBet.ID).ToList();
            foreach (var existingOdd in existingOdds)
            {
                if (!newBet.Odds.Any(o => o.ID == existingOdd.ID))
                {
                    context.Odds.Remove(existingOdd);
                }
            }
        }

        private XmlSports DeserializeFeed(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlSports));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (XmlSports)serializer.Deserialize(reader);
            }
        }
    }
}
