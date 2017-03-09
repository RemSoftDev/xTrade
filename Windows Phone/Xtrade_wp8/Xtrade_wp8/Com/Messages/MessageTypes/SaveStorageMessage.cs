using Xtrade_wp8.Com.Messages;
using Xtrade_wp8.Com.Messages.MessageData;

class SaveStorageMessage:BaseMessage
{
  /*  private SaveStorageData _data;

    public SaveStorageData Data {
        get { return _data; }
        set { _data = value; }
    }*/

   /* public string _key
    {
        get { return key; }
        set { key = value; }
    }*/
    public string _key { set; get; }
    public dynamic _obj { get; set; }
}
