using Doc_Chat.Entidades;
using MongoDB.Driver;

namespace Doc_Chat.Persistencia
{
    public class ChatContext : IChatContext
    {
        public IMongoCollection<Chat> Chats { get; }

        public ChatContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("MongoSettings:ConnectionString"));

            var database = client.GetDatabase(configuration.GetValue<string>("MongoSettings:DatabaseName"));
            Chats = database.GetCollection<Chat>(configuration.GetValue<string>("MongoSettings:CollectionName"));
        }
    }
}
