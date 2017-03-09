using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class CloseChartMessage : BaseMessage
    {
        public CloseChartData Data { get; set; }
    }
}