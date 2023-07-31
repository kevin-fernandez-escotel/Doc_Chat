using AutoMapper;
using Doc_Chat.Entidades;
using Doc_Chat.Models;
using Doc_Chat.Persistencia;
using MongoDB.Driver;

namespace Doc_Chat.Repositorios
{
    public class ChatRepository : IChatRepository
    {
        private readonly IChatContext chatContext;
        private readonly IMapper mapper;

        public ChatRepository(IChatContext chatContext, IMapper mapper)
        {
            this.chatContext = chatContext;
            this.mapper = mapper;
        }

        public async Task<List<ChatDTO>> LoadChatsCabina()
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.ChatActivo, true);

            var listaChats = await chatContext.Chats.Find(filter).ToListAsync();

            var chats = mapper.Map<List<ChatDTO>>(listaChats).OrderByDescending(x => x.FechaUltimoMensaje).ToList();

            return chats;
        }

        public async Task<List<ChatDTO>> LoadChatsProveedor(string proveedorId)
        {
            var filter = Builders<Chat>.Filter.And(
                    Builders<Chat>.Filter.Eq(c => c.ProveedorId, proveedorId),
                    Builders<Chat>.Filter.Eq(c => c.ChatActivo, true)
                );

            var listaChats = await chatContext.Chats.Find(filter).ToListAsync();

            var chats = mapper.Map<List<ChatDTO>>(listaChats).OrderByDescending(x => x.FechaUltimoMensaje).ToList();

            return chats;
        }

        public async Task<Chat> LoadMensajes(string id)
        {
            var chat = await chatContext.Chats.Find(c => c.Id == id).FirstOrDefaultAsync();

            return chat;
        }

        public async Task<Chat> LoadMensajesChatAsistencia(string asistenciaId)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.AsistenciaId, asistenciaId);

            var chat = await chatContext.Chats.Find(filter).FirstOrDefaultAsync();

            return chat;
        }

        public async Task NuevoMensaje(ReceivedMessages receivedMessages)
        {
            var mensaje = mapper.Map<Mensaje>(receivedMessages);
            mensaje.FechaMensaje = DateTime.Now;

            var filter = Builders<Chat>.Filter.Eq(c => c.AsistenciaId, receivedMessages.AsistenciaId);
            var update = Builders<Chat>.Update.Combine(
                Builders<Chat>.Update.Push(c => c.Mensajes, mensaje),
                Builders<Chat>.Update.Set(c => c.FechaUltimoMensaje, DateTime.Now)
             );

            await chatContext.Chats.UpdateOneAsync(filter, update);
        }

        public async Task NuevoChat(Chat nuevoChat)
        {
            nuevoChat.Mensajes.First().FechaMensaje = DateTime.Now;
            await chatContext.Chats.InsertOneAsync(nuevoChat);
        }

        public async Task CerrarChat(string asistenciaId)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.AsistenciaId, asistenciaId);
            var update = Builders<Chat>.Update.Set(c => c.ChatActivo, false);
            await chatContext.Chats.UpdateOneAsync(filter, update);
        }

        public async Task SetMensajesNoLeidosProveedor(string asistenciaId, bool value)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.AsistenciaId, asistenciaId);
            var update = Builders<Chat>.Update.Set(c => c.MensajesProveedorNoLeidos, value);

            await chatContext.Chats.UpdateOneAsync(filter, update);
        }

        public async Task SetMensajesNoLeidosCabina(string asistenciaId, bool value)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.AsistenciaId, asistenciaId);
            var update = Builders<Chat>.Update.Set(c => c.MensajesCabinaNoLeidos, value);

            await chatContext.Chats.UpdateOneAsync(filter, update);
        }
    }
}
