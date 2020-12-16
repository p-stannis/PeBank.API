using AutoMapper;
using PeBank.API.Entities;

namespace PeBank.API.Features
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerModel>();
            CreateMap<CustomerCreateRequest, Customer>();
        }
    }
}
