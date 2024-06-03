using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{
    public class Match
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string MatchType { get; set; }
        public int EventID { get; set; }
        public Event Event { get; set; }
        public List<Bet> Bets { get; set; }
    }
}
