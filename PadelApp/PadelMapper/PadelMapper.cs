
using AutoMapper;
using PadelApp.Helpers;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.PadelMapper
{
    public class PadelMapper : Profile
    {
        public PadelMapper()
        {
            CreateMap<Sede, SedeDto>().ReverseMap();
            CreateMap<Sede, CrearSedeDto>().ReverseMap();
            CreateMap<Sede, ModificarSedeDto>().ReverseMap();
            CreateMap<Pista, PistaDto>()
                .ForMember(dest => dest.nombreSede, opt => opt.MapFrom(src => src.Sede.nombreSede))
                .ForMember(dest => dest.direccionSede, opt => opt.MapFrom(src => src.Sede.direccion))
                .ForMember(dest => dest.estadoDescripcion,
               opt => opt.MapFrom(src => src.estado.GetDisplayName()));

            CreateMap<Pista, ModificarPistaDto>().ReverseMap();
            CreateMap<Pista, CrearPistaDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, ModificarUsuarioDto>().ReverseMap();
            CreateMap<Usuario, RegistroUsuarioDto>().ReverseMap();
            CreateMap<Reserva, ReservaDto>()
                .ForMember(dest => dest.nombreSede, opt => opt.MapFrom(src => src.Pista.Sede.nombreSede))
                .ForMember(dest => dest.direccionSede, opt => opt.MapFrom(src => src.Pista.Sede.direccion))
                .ForMember(dest => dest.nombrePista, opt => opt.MapFrom(src => src.Pista.nombrePista))
                .ForMember(dest => dest.nombreUsuario, opt => opt.MapFrom(src => src.Usuario.nombre + " " + src.Usuario.apellidos))
                .ForMember(dest => dest.estadoDescripcion, opt => opt.MapFrom(src => src.estado.ToString()))
                .AfterMap((src, dest) =>
                {
                    // Si el DTO detecta que ya pasó la hora, cambiamos la descripción
                    if (dest.EstaFinalizada)
                    {
                        dest.estadoDescripcion = "Finalizada";
                    }
                });

            CreateMap<Reserva, CrearReservaDto>().ReverseMap();
        }
    }
}
