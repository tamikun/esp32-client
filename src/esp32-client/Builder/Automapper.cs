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

            CreateMap<MachineCreateModel, Machine>();
            CreateMap<MachineNameUpdateModel, Machine>();
            CreateMap<Machine, MachineNameUpdateModel>();

            CreateMap<Product, ProductUpdateModel>();
        }
    }
}