using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    public class MongoDbClient : IClient
    {
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;
        //private IMongoCollection<ReporterBase> mongoCollection;

        public ClientStatus Status { get; private set; } = ClientStatus.Disconnected;

        public async Task Connect(CancellationToken token)
        {
            mongoClient = new MongoClient("mongodb://localhost:27017");
            mongoDatabase = mongoClient.GetDatabase("dlas");

            Status = ClientStatus.Connected;
        }

        public async Task Disconnect(CancellationToken token)
        {
            Status = ClientStatus.Disconnected;
        }

        public async Task SendValue(ReporterBase reporter, CancellationToken token)
        {
            IMongoCollection<BsonDocument>  mongoCollection = mongoDatabase.GetCollection<BsonDocument>(reporter.Context.SessionId);
            var bd = reporter.ToBsonDocument();
            await mongoCollection.InsertOneAsync(bd, cancellationToken: token);
        }
    }
}
