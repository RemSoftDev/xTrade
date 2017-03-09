using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class TrackEventMessage : BaseMessage
    {
        public EventData Data { get; set; }
    }
}