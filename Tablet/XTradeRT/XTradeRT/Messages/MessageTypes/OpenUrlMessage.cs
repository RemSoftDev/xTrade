using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class OpenUrlMessage : BaseMessage
    {
        public UrlData Data { get; set; }
    }
}