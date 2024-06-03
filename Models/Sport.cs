namespace UltraPlayBettingData.Models
{
    public class Sport
    {
        public int SportId { get; set; }
        public string Name { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
