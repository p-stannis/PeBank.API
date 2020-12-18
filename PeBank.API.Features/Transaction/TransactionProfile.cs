using AutoMapper;
using PeBank.API.Entities;

namespace PeBank.API.Features
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionCreateRequest, Transaction>();
            CreateMap<Transaction, TransactionModel>();
        }
    }
}
