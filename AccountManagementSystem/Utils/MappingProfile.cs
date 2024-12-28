using AutoMapper;
using Models;
using Models.DTOs;

namespace AccountManagementSystem.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountModelInputDTO, Account>();
            CreateMap<Account, AccountReadDTO>()
            .ForMember(
                dest => dest.PrimaryAccountHolderAdharNumber, 
                opt => opt.MapFrom(src => src.PrimaryAccountHolder.AdharNumber.Number
            ))
            .ForMember(
                dest => dest.SecondaryAccountHolderAdharNumber, 
                opt => opt.MapFrom(src => src.SecondaryAccountHolder.AdharNumber.Number
            ))
            .ForMember(
                dest => dest.NomineeAdharNumber, 
                opt => opt.MapFrom(src => src.Nominee.AdharNumber.Number
            ));
            
            // CreateMap<UserAdharReadDTO, Account>().ReverseMap();
        }
    }
}