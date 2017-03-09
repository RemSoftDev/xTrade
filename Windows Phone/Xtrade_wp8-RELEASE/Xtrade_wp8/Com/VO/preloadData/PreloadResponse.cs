using System;
using System.Collections.Generic;
using Xtrade_wp8.Com.VO.preloadData;
namespace Xtrade_wp8.Com.VO
{
    public class PreloadResponse
    {
        public string language { get; set; }
        public List<AppVersion> appVersion { get; set; }
        public DataVersion dataVersion  { get; set; }
    }
}
