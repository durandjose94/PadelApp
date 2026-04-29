namespace PadelApp.Modelos.Dtos
{
    public class AnuncioDto
    {
        public int idAnuncio { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; } // Extraído de la relación con Usuario
        public TipoAnuncio tipoAnuncio { get; set; }
        public string tipoAnuncioDescripcion { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public decimal? nivelRequerido { get; set; }
        public string nivelRequeridoDescripcion { get; set; }
        public DateTime? fechaEvento { get; set; }
        public DateTime fechaExpiracion { get; set; }
        public string telefonoContacto { get; set; }
        public bool permiteWhatsapp { get; set; }
        public bool permiteLlamada { get; set; }
        public decimal? precio { get; set; }
        public DateTime fecha_registro { get; set; }

        // Campo extra muy útil para el Front
        public bool esVigente => fechaExpiracion >= DateTime.Now;
    }
}
