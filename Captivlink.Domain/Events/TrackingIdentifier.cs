using Captivlink.Infrastructure.Utility;

namespace Captivlink.Infrastructure.Events
{
    public class TrackingIdentifier
    {
        public string UserAgentHash { get; }
        public string SessionId { get; }
        public Guid CampaignCreatorId { get; }

        private TrackingIdentifier(string userAgentHash, string sessionId, Guid campaignCreatorId )
        {
            UserAgentHash = userAgentHash;
            SessionId = sessionId;
            CampaignCreatorId = campaignCreatorId;
        }

        public static TrackingIdentifier? Parse(string value)
        {
            if (DependencyInjection.EncryptionKey == null) return null;

            var parsed = AesOperation.DecryptString(DependencyInjection.EncryptionKey, value);
            var splitted = parsed.Split('|');
            if (splitted.Length != 3)
            {
                return null;
            }

            if (!Guid.TryParse(splitted[2], out Guid id))
            {
                return null;
            }

            return new TrackingIdentifier(splitted[0], splitted[1], Guid.Parse(splitted[2]));
        }

        private static string? Encode(TrackingIdentifier trackingIdentifier)
        {
            if (DependencyInjection.EncryptionKey == null) return null;

            var value =
                $"{trackingIdentifier.UserAgentHash}|{trackingIdentifier.SessionId}|{trackingIdentifier.CampaignCreatorId}";

            return AesOperation.EncryptString(DependencyInjection.EncryptionKey, value);
        }

        public static string? Create(string userAgent, Guid campaignCreatorId)
        {

            var userAgentHash = HashOperation.GetHashString(userAgent);
            var sessionId = Guid.NewGuid().ToString("N") + "-1";
            return Encode(new TrackingIdentifier(userAgentHash, sessionId, campaignCreatorId));
        }

        public static string? Next(TrackingIdentifier previousValue)
        {
            var sessionId = previousValue.SessionId.Split("-");
            if (sessionId.Length != 2)
            {
                return null;
            }

            if (!int.TryParse(sessionId[1], out int step))
            {
                return null;
            }

            step++;
            return Encode(new TrackingIdentifier(previousValue.UserAgentHash, sessionId[0] + "-" + step,
                previousValue.CampaignCreatorId));
        }

        public static bool CheckIfValid(string encodedValue, string requestUserAgent, Guid campaignCreatorId)
        {
            var decoded = Parse(encodedValue);
            if (decoded == null)
                return false;

            if (decoded.CampaignCreatorId != campaignCreatorId)
            {
                return false;
            }

            if (string.IsNullOrEmpty(decoded.SessionId))
            {
                return false;
            }

            var userAgentHash = HashOperation.GetHashString(requestUserAgent);
            return userAgentHash == decoded.UserAgentHash;
        }
    }
}
