using AutoMapper;
using PeBank.API.Entities;

namespace PeBank.API.Features
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountCreateRequest, Account>();
            CreateMap<Account, AccountModel>();
        }
    }
}
