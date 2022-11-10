using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models.Requests;
using CardStorageService.Models;

namespace CardStorageService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Card, CardDto>();
            CreateMap<CreateCardRequest, Card>();
            CreateMap<CreateClientRequest, Client>();
        }

    }
}
