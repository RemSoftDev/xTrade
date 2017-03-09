using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class CloseChartMessage : BaseMessage
    {
        public CloseChartData Data { get; set; }
    }
}