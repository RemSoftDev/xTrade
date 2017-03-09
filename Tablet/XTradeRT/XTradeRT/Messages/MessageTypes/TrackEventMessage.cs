using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class TrackEventMessage : BaseMessage
    {
        public EventData Data { get; set; }
    }
}