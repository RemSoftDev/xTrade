using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class ChartInfoMessage : BaseMessage
    {
        public ChartInfo Data { get; set; }
    }
}