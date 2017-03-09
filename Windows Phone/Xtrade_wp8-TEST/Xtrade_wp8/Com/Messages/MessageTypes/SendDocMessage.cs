using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class SendDocMessage : BaseMessage
    {
        public SendDocData Data { get; set; }
    }
}
