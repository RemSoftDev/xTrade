using Xtrade_wp8.Com.VO;

namespace Xtrade_wp8.Com.Messages
{
    public class SystemParamsMessage
    {
        public string DefaultLanguage { get; set; }

        public string OsId { get; set; }

        public string DeviceId { get; set; }

        public string pushwooshToken { get; set; }

        public dynamic localStorage { get; set; }
    }
}