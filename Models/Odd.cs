namespace UltraPlayBettingData.Models
{
    public class Odd
    {
        public int Id { get; set; }
        public int BetId { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public float? SpecialBetValue { get; set; }
        public Bet Bet { get; set; }
    }
}
