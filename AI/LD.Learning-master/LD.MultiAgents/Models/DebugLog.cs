using System.Security.Cryptography;

namespace LD.MultiAgents.Models
{
    public record DebugLog
    {

        public string Id { get; set; }
        public string MessageId { get; set; }

        public string Type { get; set; }

        public string SessionId { get; set; }

        public string TenantId { get; set; }

        public string UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        

        public DebugLog(string tenantId, string userId, string sessionId, string messageId, string id)
        {
            SessionId = sessionId;
            MessageId = messageId;
            TenantId = tenantId;
            UserId = userId;
            Id = id;
            Type = nameof(DebugLog);
            TimeStamp = DateTime.UtcNow;
            
        }
    }
}
