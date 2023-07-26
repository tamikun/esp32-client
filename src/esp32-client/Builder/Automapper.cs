using AutoMapper;
using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Builder
{
    public class Automapper : Profile
    {
        public Automapper()
        {
            CreateMap<UserAccountCreateModel, UserAccount>();

            CreateMap<PatternCreateModel, Pattern>();
            CreateMap<PatternUpdateModel, Pattern>();
            
            CreateMap<Pattern, PatternUpdateModel>();

            CreateMap<MachineCreateModel, Machine>();
            CreateMap<MachineUpdateModel, Machine>();

            CreateMap<Product, ProductUpdateModel>();
        }
    }
}