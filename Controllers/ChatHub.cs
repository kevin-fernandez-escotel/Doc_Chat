using Doc_Chat.Entidades;
using Doc_Chat.Models;
using Doc_Chat.Repositorios;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Doc_Chat.Controllers
{
    /// <summary>
    /// This class is used to communicate the client with the server using websockets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class inherits of <see cref="Hub"/>
    /// </para>
    /// <list type="table">
    /// <item>
    /// <term>Hub</term>
    /// <description>
    /// This class provides us with the necessary tools to use SignalR
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ChatHub : Hub
    {
        private readonly IChatRepository chatRepository;

        public ChatHub(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        #region .      Enviar Mensajes       .
        /// <summary>
        /// This method sends the message to others, sets the MensajesCabinaNoLeidos to true
        /// </summary>
        /// <param name="receivedMessages">The <see cref="ReceivedMessages"/> contains the data to send the message</param>
        public async Task SendMensajeProveedor(ReceivedMessages receivedMessages)
        {
            await chatRepository.NuevoMensaje(receivedMessages);
            await chatRepository.SetMensajesNoLeidosCabina(receivedMessages.AsistenciaId, true);

            await Clients.Others.SendAsync("message", receivedMessages);
        }

        /// <summary>
        /// This method sends the message to others, sets the MensajesProveedorNoLeidos to true
        /// </summary>
        /// <param name="receivedMessages">The <see cref="ReceivedMessages"/> contains the data to send the message</param>
        public async Task SendMensajeCabina(ReceivedMessages receivedMessages)
        {
            await chatRepository.NuevoMensaje(receivedMessages);
            await chatRepository.SetMensajesNoLeidosProveedor(receivedMessages.AsistenciaId, true);

            await Clients.Others.SendAsync("message", receivedMessages);
        }
        #endregion

        #region .       Cargar Chats       .
        /// <summary>
        /// This method loads all active chats from Cabina to de caller, this means all chats with active conversation and an active asistencia 
        /// </summary>
        /// <returns></returns>
        public async Task LoadChatsCabina()
        {
            var chats = await chatRepository.LoadChatsCabina();

            await Clients.Caller.SendAsync("loadChatsCabina", chats);
        }

        /// <summary>
        /// This method loads all active chats from Proveedores to de caller, this means all chats with active conversation and an active asistencia 
        /// </summary>
        /// <returns></returns>
        public async Task LoadChatsProveedor(string proveedorId)
        {
            var chats = await chatRepository.LoadChatsProveedor(proveedorId);

            await Clients.Caller.SendAsync("loadChatsProveedor", chats);
        }
        #endregion

        #region .       Contar Chats No Leidos       .
        /// <summary>
        /// This method gives the caller the count of chats that Cabina has not been opened.
        /// </summary>
        /// <returns></returns>
        public async Task CountChatsCabinaNoLeidos()
        {
            var chats = await chatRepository.LoadChatsCabina();

            var countChats = chats.Count(x => x.MensajesCabinaNoLeidos);
            await Clients.Caller.SendAsync("countChatsCabinaNoLeidos", countChats);
        }

        /// <summary>
        /// This method gives the caller the count of chats that Proveedores has not been opened.
        /// </summary>
        /// <returns></returns>
        public async Task CountChatsProveedorNoLeidos(string proveedorId)
        {
            var chats = await chatRepository.LoadChatsProveedor(proveedorId);

            var countChats = chats.Count(x => x.MensajesProveedorNoLeidos);
            await Clients.Caller.SendAsync("countChatsProveedorNoLeidos", countChats);
        }
        #endregion

        #region .      Cargar Mensajes       .
        /// <summary>
        /// This method gives the caller all the messages of the specified chat id.
        /// </summary>
        /// <param name="id">Chat Id</param>
        /// <returns></returns>
        public async Task LoadMensajes(string id)
        {
            var chat = await chatRepository.LoadMensajes(id);

            await Clients.Caller.SendAsync("loadMensajes", chat);
        }

        /// <summary>
        /// This method gives the caller all the messages of the specified asistencia id.
        /// </summary>
        /// <param name="asistenciaId">asistencia Id</param>
        /// <returns></returns>
        public async Task LoadMensajesChatAsistencia(string asistenciaId)
        {
            var chat = await chatRepository.LoadMensajesChatAsistencia(asistenciaId);

            await Clients.Caller.SendAsync("loadMensajes", chat);
        }
        #endregion

        #region .       Actualizar estatus de chats no leidos       .
        /// <summary>
        /// This method set the MensajesNoLeidosCabina to true of the specified asistencia id
        /// </summary>
        /// <param name="asistenciaId">asistencia Id</param>
        /// <returns></returns>
        public async Task UpdateChatCabinaNoLeido(string asistenciaId)
        {
            await chatRepository.SetMensajesNoLeidosCabina(asistenciaId, false);
        }

        /// <summary>
        /// This method set the MensajesNoLeidosCabina to true of the specified asistencia id
        /// </summary>
        /// <param name="asistenciaId">asistencia Id</param>
        /// <returns></returns>
        public async Task UpdateChatProveedorNoLeido(string asistenciaId)
        {
            await chatRepository.SetMensajesNoLeidosProveedor(asistenciaId, false);
        }
        #endregion

        #region  .     Nuevo Chat     .
        /// <summary>
        /// This method creates a new chat in the data base and notifies to all conections the new chat existence
        /// </summary>
        /// <param name="nuevoChat">The <see cref="Chat"/> contains the data to create a new chat</param>
        /// <returns></returns>
        public async Task NuevoChat(Chat nuevoChat)
        {
            await chatRepository.NuevoChat(nuevoChat);

            var newChat = new
            {
                nuevoChat.Id,
                nuevoChat.AsistenciaId,
                nuevoChat.ProveedorId,
                nuevoChat.MensajesProveedorNoLeidos,
                nuevoChat.MensajesCabinaNoLeidos,
                Mensaje = nuevoChat.Mensajes.Last()
            };

            await Clients.All.SendAsync("nuevoChat", newChat);
        }
        #endregion

        #region  .     CerrarChat     .
        /// <summary>
        /// This method set to false the property chatActivo and reload all chats from Cabina and Proveedores to update the list
        /// </summary>
        /// <param name="asistenciaId">Asistencia Id</param>
        /// <param name="proveedorId">Proveedor Id</param>
        /// <returns></returns>
        public async Task CerrarChat(string asistenciaId, string proveedorId)
        {
            await chatRepository.CerrarChat(asistenciaId);

            var chatsProveedor = await chatRepository.LoadChatsProveedor(proveedorId);
            await Clients.Others.SendAsync("loadChatsProveedor", chatsProveedor);

            var chats = await chatRepository.LoadChatsCabina();
            await Clients.Others.SendAsync("loadChatsCabina", chats);
        }
        #endregion

        #region  .     Recibir mensaje tracking proveedor     .
        /// <summary>
        /// This method send a new chat with de url of the tracking service with the specified asistenciaId 
        /// </summary>
        /// <param name="nuevoChat">The <see cref="Chat"/> contains the data to create a new chat</param>
        /// <returns></returns>
        public async Task RecibirMensajeTrackingProveedor(Chat nuevoChat)
        {
            await chatRepository.NuevoChat(nuevoChat);

            var newChat = new
            {
                nuevoChat.Id,
                nuevoChat.AsistenciaId,
                nuevoChat.ProveedorId,
                nuevoChat.MensajesProveedorNoLeidos,
                nuevoChat.MensajesCabinaNoLeidos,
                Mensaje = nuevoChat.Mensajes.Last()
            };

            await Clients.All.SendAsync("nuevoChat", newChat);
        }
        #endregion

        #region  .     Somebody typing     .
        /// <summary>
        /// This method notifies all existing connections that someone is typing in the chat to the specified asistenciaId
        /// </summary>
        /// <param name="userName">The username from de User who is typing</param>
        /// <param name="asistenciaId">Asistencia Id</param>
        /// <returns></returns>
        public async Task AlguienEscribiendo(string userName, string asistenciaId)
        {
            await Clients.Others.SendAsync("alguienEscribiendo", userName, asistenciaId);
        }

        /// <summary>
        /// This method notifies all existing connections that someone stop typing in the chat to the specified asistenciaId
        /// </summary>
        /// <param name="asistenciaId">Asistencia Id</param>
        /// <returns></returns>
        public async Task StopTyping(string asistenciaId)
        {
            await Clients.Others.SendAsync("stopTyping", asistenciaId);
        }
        #endregion
    }
}
