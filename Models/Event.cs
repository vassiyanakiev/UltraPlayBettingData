namespace UltraPlayBettingData.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
