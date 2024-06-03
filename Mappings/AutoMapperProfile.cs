using System.Text.RegularExpressions;
using UltraPlayBettingData.Models;
using UltraPlayBettingData.DTO_s;

namespace UltraPlayBettingData.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Match, MatchDto>()
                .ForMember(dest => dest.MatchName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.ActivePreviewBets, opt => opt.MapFrom(src => src.Bets.Where(b => !b.IsLive)));

            CreateMap<Bet, BetDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsLive, opt => opt.MapFrom(src => src.IsLive))
                .ForMember(dest => dest.Odds, opt => opt.MapFrom(src => src.Odds));

            CreateMap<Odd, OddDto>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.SpecialBetValue, opt => opt.MapFrom(src => src.SpecialBetValue));
        }
    }
}
