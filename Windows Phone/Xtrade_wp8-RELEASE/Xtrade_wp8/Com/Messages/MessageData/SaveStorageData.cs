using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtrade_wp8.Com.Messages.MessageData
{
    class SaveStorageData
    {
        private string key;
        private dynamic obj;

        public string _key {
            get {return key;}
            set { key = value; }
        }

        public dynamic _obj { get; set; }
    }
}
