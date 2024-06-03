namespace UltraPlayBettingData.Models
{
    public class Odd
    {
        public int OddId { get; set; }
        public int BetId { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string SpecialBetValue { get; set; }
        public Bet Bet { get; set; }
    }
}
