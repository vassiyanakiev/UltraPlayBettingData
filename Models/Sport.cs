using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{
    public class Sport
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Event> Events { get; set; }
    }
}
