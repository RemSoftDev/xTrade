using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class ChartInfoMessage : BaseMessage
    {
        public ChartInfo Data { get; set; }
    }
}