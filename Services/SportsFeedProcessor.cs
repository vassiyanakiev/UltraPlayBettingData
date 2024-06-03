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

                    // Save to database
                    context.Add(xmlSports);
                    await context.SaveChangesAsync();

                    // Add update messages
                    updateServiceValue.AddUpdateMessage(nameof(Sport), "Add", xmlSports.Sport);
                    foreach (var @event in xmlSports.Sport.Events)
                    {
                        updateServiceValue.AddUpdateMessage(nameof(Event), "Add", @event);
                        foreach (var match in @event.Matches)
                        {
                            updateServiceValue.AddUpdateMessage(nameof(Match), "Add", match);
                            foreach (var bet in match.Bets)
                            {
                                updateServiceValue.AddUpdateMessage(nameof(Bet), "Add", bet);
                                foreach (var odd in bet.Odds)
                                {
                                    updateServiceValue.AddUpdateMessage(nameof(Odd), "Add", odd);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
