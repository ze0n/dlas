using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    [DataContract]
    public class ReporterBase
    {
        [BsonIgnore]
        private readonly ISessionManager sessionManager;

        public ReporterBase(ISessionManager agent)
        {
            Context = new ValueContext();
            RepresentationInfo = new ValueRepresentationInfo();
            ValueInfo = new ValueInfo();
            this.sessionManager = agent ?? throw new System.ArgumentNullException(nameof(agent));
        }

        public ValueContext Context { get; set; }
        public ValueInfo ValueInfo { get; set; }
        public ValueRepresentationInfo RepresentationInfo { get; set; }

        public void Send()
        {
            sessionManager.SendValue(this);
        }

        public Task SendAsync(CancellationToken token)
        {
            return sessionManager.SendValue(this, token);
        }
    }
}
