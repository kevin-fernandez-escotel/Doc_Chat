using AutoMapper;
using Doc_Chat.Entidades;
using Doc_Chat.Models;

namespace Doc_Chat.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ReceivedMessages, Mensaje>();
            CreateMap<Chat, ChatDTO>()
                .ForMember(x => x.Mensaje, dto => dto.MapFrom(campo => campo.Mensajes.Last()));
        }
    }
}
