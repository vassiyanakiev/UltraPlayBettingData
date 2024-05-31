namespace UltraPlayBettingData.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Match Match { get; set; }
        public ICollection<Odd> Odds { get; set; }
    }
}
