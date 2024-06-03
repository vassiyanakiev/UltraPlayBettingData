

using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{   public class Event
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
        public int CategoryID { get; set; }
        public int SportID { get; set; }
        public Sport Sport { get; set; }
        public List<Match> Matches { get; set; }
    }
}
