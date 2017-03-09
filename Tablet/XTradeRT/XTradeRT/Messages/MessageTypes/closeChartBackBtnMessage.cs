using XTradeRT.Messages.MessageData;

namespace XTradeRT.Messages.MessageTypes
{
    class CloseChartBackBtnMessage : BaseMessage
    {
        public CloseChartData Data { get; set; }
    }
}
