using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{
    public class Odd
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal SpecialBetValue { get; set; }
        public int BetID { get; set; }
        public Bet Bet { get; set; }
    }
}
