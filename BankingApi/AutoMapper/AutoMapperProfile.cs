namespace BankingApi.AutoMapper
{
    using BankingApi.Data.Entities;
    using BankingApi.Models.Dto;
    using BankingApi.Models.Requests;
    using global::AutoMapper;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ClientCreateRequest, Client>();
            CreateMap<AddressDto, Address>();
            CreateMap<AccountDto, Account>();
        }
    }
}
