using AutoMapper;

namespace PeBank.API.Features
{
    public class StatementProfile : Profile
    {
        public StatementProfile()
        {
            CreateMap<AccountModel, StatementModel>()
                .ForMember(dest => dest.AccountId, opts => opts.MapFrom(src => src.Id));
        }
    }
}
