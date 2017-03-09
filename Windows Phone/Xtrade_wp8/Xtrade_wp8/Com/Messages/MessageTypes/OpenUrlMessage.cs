using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class OpenUrlMessage : BaseMessage
    {
        public UrlData Data { get; set; }
    }
}