namespace Doc_Chat.Models
{
    public class ReceivedMessages
    {
        public string AsistenciaId { get; set; }
        public string EmisorId { get; set; }
        public string EmisorNombre { get; set; }
        public int TipoMensaje { get; set; }
        public string Texto { get; set; }
        public string UrlArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoUsuario { get; set; }
    }
}
