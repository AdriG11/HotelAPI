using AutoMapper;
using ClasesModelos;
using ClasesModelos.Dto;

namespace HotelAPI
{
    public class MapConfig : Profile
    {
        public MapConfig() 
        {
            CreateMap<Habitaciones, HabitacionesDto>().ReverseMap();
            CreateMap<Habitaciones, HabitacionesCreateDto>().ReverseMap();
            CreateMap<Habitaciones, HabitacionesUpdateDto>().ReverseMap();
            CreateMap<Reservas, ReservasDto>().ReverseMap();
            CreateMap<Reservas, ReservasCreateDto>().ReverseMap();
            CreateMap<Reservas, ReservasUpdateDto>().ReverseMap();
        }
    }
}
