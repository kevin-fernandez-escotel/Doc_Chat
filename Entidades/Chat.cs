using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Doc_Chat.Entidades
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AsistenciaId { get; set; }
        public string ProveedorId { get; set; }
        public DateTime FechaUltimoMensaje { get; set; } = DateTime.Now;
        public bool MensajesProveedorNoLeidos { get; set; }
        public bool MensajesCabinaNoLeidos { get; set; }
        public bool ChatActivo { get; set; }
        public List<Mensaje> Mensajes { get; set; }
    }
}
