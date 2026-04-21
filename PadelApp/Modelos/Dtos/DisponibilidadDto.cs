namespace PadelApp.Modelos.Dtos
{
    public class BloqueHorarioDto
    {
        public string Hora { get; set; }
        public bool Disponible { get; set; }
        public bool EsPasada { get; set; }
    }
    public class DisponibilidadDto
    {
        public int IdPista { get; set; }
        public DateOnly Fecha { get; set; }
        public List<BloqueHorarioDto> Horarios { get; set; }
    }
}
