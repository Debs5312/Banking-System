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
        }
    }
}