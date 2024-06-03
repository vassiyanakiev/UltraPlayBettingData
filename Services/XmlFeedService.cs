using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using UltraPlayBettingData.Data;
using UltraPlayBettingData.Models;

namespace UltraPlayBettingData.Services
{
    public class XmlFeedService : BackgroundService
    {
        private readonly IHttpClientFactory httpClientFactoryValue;
        private readonly IServiceScopeFactory scopeFactoryValue;
        private readonly string feedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=80E2CA86-3F7E-4D82-936C-05CC05C24A2B&sportId=2357&days=7";

        public XmlFeedService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory)
        {
            httpClientFactoryValue = httpClientFactory;
            scopeFactoryValue = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchAndProcessFeed();
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

        private async Task FetchAndProcessFeed()
        {
            var httpClient = httpClientFactoryValue.CreateClient();
            var response = await httpClient.GetStringAsync(feedUrl);

            if (!string.IsNullOrEmpty(response))
            {
                var xmlDocument = XDocument.Parse(response);
                await ProcessFeed(xmlDocument);
            }
        }

        private async Task ProcessFeed(XDocument xmlDocument)
        {
            using (var scope = scopeFactoryValue.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BettingContext>();

                // Parse and process XML data
                foreach (var sportElement in xmlDocument.Descendants("Sport"))
                {
                    var sport = new Sport
                    {
                        // Assuming the Sport element has an Id and Name attributes
                        SportId = int.Parse(sportElement.Attribute("Id").Value),
                        Name = sportElement.Attribute("Name").Value,
                        Events = sportElement.Descendants("Event").Select(e => new Event
                        {
                            EventId = int.Parse(e.Attribute("Id").Value),
                            Name = e.Attribute("Name").Value,
                            Matches = e.Descendants("Match").Select(m => new Match
                            {
                                MatchId = int.Parse(m.Attribute("Id").Value),
                                Name = m.Attribute("Name").Value,
                                StartDate = DateTime.Parse(m.Attribute("StartDate").Value),
                                MatchType = m.Attribute("MatchType").Value,
                                Bets = m.Descendants("Bet").Select(b => new Bet
                                {
                                    BetId = int.Parse(b.Attribute("Id").Value),
                                    Name = b.Attribute("Name").Value,
                                    IsActive = bool.Parse(b.Attribute("IsActive").Value),
                                    Odds = b.Descendants("Odd").Select(o => new Odd
                                    {
                                        OddId = int.Parse(o.Attribute("Id").Value),
                                        Value = decimal.Parse(o.Attribute("Value").Value),
                                        SpecialBetValue = o.Attribute("SpecialBetValue")?.Value
                                    }).ToList()
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    };

                    // Add or update the sport in the database
                    var existingSport = await dbContext.Sports
                        .Include(s => s.Events)
                        .ThenInclude(e => e.Matches)
                        .ThenInclude(m => m.Bets)
                        .ThenInclude(b => b.Odds)
                        .FirstOrDefaultAsync(s => s.SportId == sport.SportId);

                    if (existingSport == null)
                    {
                        dbContext.Sports.Add(sport);
                    }
                    else
                    {
                        // Update existing sport with new data
                        dbContext.Entry(existingSport).CurrentValues.SetValues(sport);

                        foreach (var existingEvent in existingSport.Events)
                        {
                            var newEvent = sport.Events.FirstOrDefault(e => e.EventId == existingEvent.EventId);
                            if (newEvent == null)
                            {
                                dbContext.Events.Remove(existingEvent);
                            }
                            else
                            {
                                dbContext.Entry(existingEvent).CurrentValues.SetValues(newEvent);

                                // Update matches
                                foreach (var existingMatch in existingEvent.Matches)
                                {
                                    var newMatch = newEvent.Matches.FirstOrDefault(m => m.MatchId == existingMatch.MatchId);
                                    if (newMatch == null)
                                    {
                                        dbContext.Matches.Remove(existingMatch);
                                    }
                                    else
                                    {
                                        dbContext.Entry(existingMatch).CurrentValues.SetValues(newMatch);

                                        // Update bets
                                        foreach (var existingBet in existingMatch.Bets)
                                        {
                                            var newBet = newMatch.Bets.FirstOrDefault(b => b.BetId == existingBet.BetId);
                                            if (newBet == null)
                                            {
                                                dbContext.Bets.Remove(existingBet);
                                            }
                                            else
                                            {
                                                dbContext.Entry(existingBet).CurrentValues.SetValues(newBet);

                                                // Update odds
                                                foreach (var existingOdd in existingBet.Odds)
                                                {
                                                    var newOdd = newBet.Odds.FirstOrDefault(o => o.OddId == existingOdd.OddId);
                                                    if (newOdd == null)
                                                    {
                                                        dbContext.Odds.Remove(existingOdd);
                                                    }
                                                    else
                                                    {
                                                        dbContext.Entry(existingOdd).CurrentValues.SetValues(newOdd);
                                                    }
                                                }

                                                // Add new odds
                                                foreach (var newOdd in newBet.Odds)
                                                {
                                                    if (!existingBet.Odds.Any(o => o.OddId == newOdd.OddId))
                                                    {
                                                        existingBet.Odds.Add(newOdd);
                                                    }
                                                }
                                            }
                                        }
                                        // Add new bets
                                        foreach (var newBet in newMatch.Bets)
                                        {
                                            if (!existingMatch.Bets.Any(b => b.BetId == newBet.BetId))
                                            {
                                                existingMatch.Bets.Add(newBet);
                                            }
                                        }

                                    }
                                }

                                // Add new matches
                                foreach (var newMatch in newEvent.Matches)
                                {
                                    if (!existingEvent.Matches.Any(m => m.MatchId == newMatch.MatchId))
                                    {
                                        existingEvent.Matches.Add(newMatch);
                                    }
                                }


                            }
                        }
                        // Add new events
                        foreach (var newEvent in sport.Events)
                        {
                            if (!existingSport.Events.Any(e => e.EventId == newEvent.EventId))
                            {
                                existingSport.Events.Add(newEvent);
                            }
                        }
                    }

                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
