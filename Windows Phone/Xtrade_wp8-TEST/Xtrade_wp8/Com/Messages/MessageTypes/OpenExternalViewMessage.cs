using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class OpenExternalViewMessage : BaseMessage
    {
        public ExternalViewData Data { get; set; }
    }
}