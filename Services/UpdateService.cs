using System.Collections.Concurrent;

namespace UltraPlayBettingData.Services
{
    public class UpdateService
    {
        private readonly ConcurrentQueue<UpdateMessage> updateMessages = new ConcurrentQueue<UpdateMessage>();

        public void AddUpdateMessage(string entityType, string action, object entity)
        {
            updateMessages.Enqueue(new UpdateMessage { EntityType = entityType, Action = action, Entity = entity });
        }

        public IEnumerable<UpdateMessage> GetUpdateMessages()
        {
            while (updateMessages.TryDequeue(out var message))
            {
                yield return message;
            }
        }
    }
}
