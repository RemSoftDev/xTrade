using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xtrade_wp8.Com.VO;

namespace Xtrade_wp8.Com.Service
{
    class ProxyHttpConnect : INotifyPropertyChanged
    {
        private static ProxyHttpConnect instance = null;

        private ProxyHttpConnect()
        {
        }


         public event PropertyChangedEventHandler PropertyChanged;

        public static ProxyHttpConnect Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProxyHttpConnect();
                }
                return instance;
            }
        }


        public async Task<PreloadResponse> getPreloadData(string deviceID)
        {
            PreloadResponse preloadData = new PreloadResponse();

            string servUrl = GetServerUrl();
            dynamic data = deviceID.Length > 0 ? new { deviceID = deviceID.ToString() } : new Object();

            HttpClient client = new HttpClient();
            StringContent queryString = new StringContent("bla");//data.ToString()
            Uri requestStr = new Uri(servUrl + "/mw/app/pre-login");
            HttpResponseMessage response = await client.PostAsync(requestStr, queryString);
            string preloadResponse = await response.Content.ReadAsStringAsync();
            PreloadResponse pPreloadResponse = JsonConvert.DeserializeObject<PreloadResponse>(preloadResponse as String);


            return preloadData;
        }



        public string GetServerUrl()
        {
            string address = Constants.IS_DEBUG ? Constants.SERVER_ADDRESS_TEST_PREF : Constants.SERVER_ADDRESS_DEV_PREF;
            return string.Format("{0}{1}.{2}", Constants.SERVER_ADDRESS_PROTOCOL, address, Constants.SERVER_ADDRESS);
        }


       
    }
}
