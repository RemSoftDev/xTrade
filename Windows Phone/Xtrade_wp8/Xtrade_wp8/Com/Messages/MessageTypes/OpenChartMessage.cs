using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class OpenChartMessage : BaseMessage
    {
        public ChartData Data { get; set; }
    }
}