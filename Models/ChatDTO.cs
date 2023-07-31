using Doc_Chat.Entidades;

namespace Doc_Chat.Models
{
    public class ChatDTO
    {
        public string Id { get; set; }
        public string AsistenciaId { get; set; }
        public string ProveedorId { get; set; }
        public DateTime FechaUltimoMensaje { get; set; }
        public bool MensajesProveedorNoLeidos { get; set; }
        public bool MensajesCabinaNoLeidos { get; set; }
        public Mensaje Mensaje { get; set; }
    }
}
