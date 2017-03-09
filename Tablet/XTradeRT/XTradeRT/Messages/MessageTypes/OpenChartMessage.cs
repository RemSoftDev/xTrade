using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class OpenChartMessage : BaseMessage
    {
        public ChartData Data { get; set; }
    }
}