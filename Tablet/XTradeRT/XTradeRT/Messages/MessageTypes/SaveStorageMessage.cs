namespace XTradeRT.Messages.MessageTypes
{
    public class SaveStorageMessage:BaseMessage
    {
        public string _key { set; get; }
        public dynamic _obj { get; set; }
    }
}