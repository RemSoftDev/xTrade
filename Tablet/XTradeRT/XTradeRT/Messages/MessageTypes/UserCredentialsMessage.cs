using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    public class UserCredentialsMessage : BaseMessage
    {
        public UserCredentialsData Data { get; set; }
    }
}