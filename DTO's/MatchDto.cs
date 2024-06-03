namespace UltraPlayBettingData.DTO_s
{
    public class MatchDto
    {
        public string MatchName { get; set; }
        public DateTime StartDate { get; set; }
        public List<BetDto> ActivePreviewBets { get; set; }
    }

    public class BetDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<OddDto> Odds { get; set; }
    }

    public class OddDto
    {
        public decimal Value { get; set; }
        public string SpecialBetValue { get; set; }
    }

}
