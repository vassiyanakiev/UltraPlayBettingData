namespace UltraPlayBettingData.Services
{
    public class FeedBackgroundService : BackgroundService
    {
        private readonly SportsFeedProcessor sportFeedProcessor;

        public FeedBackgroundService(SportsFeedProcessor feedProcessor)
        {
            sportFeedProcessor = feedProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await sportFeedProcessor.FetchAndSaveFeed();

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
