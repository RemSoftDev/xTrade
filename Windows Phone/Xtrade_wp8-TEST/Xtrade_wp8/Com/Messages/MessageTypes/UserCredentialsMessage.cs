using Xtrade_wp8.Com.Messages.MessageData;

namespace Xtrade_wp8.Com.Messages.MessageTypes
{
    public class UserCredentialsMessage : BaseMessage
    {
        public UserCredentialsData Data { get; set; }
    }
}