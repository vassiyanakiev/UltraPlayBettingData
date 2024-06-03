using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{
    public class Bet
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
        public int MatchID { get; set; }
        public Match Match { get; set; }
        public List<Odd> Odds { get; set; }
    }
}
