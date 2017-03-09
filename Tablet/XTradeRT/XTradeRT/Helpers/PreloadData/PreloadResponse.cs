using System.Collections.Generic;

namespace XTradeRT.Helpers.PreloadData
{
    public class PreloadResponse
    {
        public string language { get; set; }
        public List<AppVersion> appVersion { get; set; }
        public DataVersion dataVersion { get; set; }
    }
}
