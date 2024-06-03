using System.ComponentModel.DataAnnotations;

namespace UltraPlayBettingData.Models
{
    public class XmlSports
    {
        [Key]
        public DateTime CreateDate { get; set; }
        public Sport Sport { get; set; }
    }
}
