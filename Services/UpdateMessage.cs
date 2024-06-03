namespace UltraPlayBettingData.Services
{
    public class UpdateMessage
    {
        public string EntityType { get; set; }
        public string Action { get; set; } // e.g., "Add", "Update", "Delete"
        public object Entity { get; set; }
    }
}
