using Doc_Chat.Entidades;
using Doc_Chat.Models;

namespace Doc_Chat.Repositorios
{
    public interface IChatRepository
    {
        Task CerrarChat(string asistenciaId);
        Task<List<ChatDTO>> LoadChatsCabina();
        Task<List<ChatDTO>> LoadChatsProveedor(string proveedorId);
        Task<Chat> LoadMensajes(string id);
        Task<Chat> LoadMensajesChatAsistencia(string asistenciaId);
        Task NuevoChat(Chat nuevoChat);
        Task NuevoMensaje(ReceivedMessages receivedMessages);
        Task SetMensajesNoLeidosCabina(string asistenciaId, bool value);
        Task SetMensajesNoLeidosProveedor(string asistenciaId, bool value);
    }
}
