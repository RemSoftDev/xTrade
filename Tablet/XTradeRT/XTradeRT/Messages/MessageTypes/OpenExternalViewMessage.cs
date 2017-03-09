using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class OpenExternalViewMessage : BaseMessage
    {
        public ExternalViewData Data { get; set; }
    }
}