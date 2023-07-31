using Doc_Chat.Entidades;
using MongoDB.Driver;

namespace Doc_Chat.Persistencia
{
    public interface IChatContext
    {
        IMongoCollection<Chat> Chats { get; }
    }
}
