using AutoMapper;
using Models;
using Models.DTOs;

namespace AccountManagementSystem.UnitTests.Fixtures
{
    public class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<AccountModelInputDTO, Account>();
        }
    }
}