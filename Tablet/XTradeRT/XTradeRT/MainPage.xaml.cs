using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Xtrade_wp8.Com;
using XTradeRT.Helpers.PreloadData;
using XTradeRT.Messages;
using XTradeRT.Messages.MessageTypes;

namespace XTradeRT
{
    public sealed partial class MainPage
    {
        #region Fields

        private bool autologin;

        private string deviceID;

        private string mainUrl;

        //private ChartsData chartsData;

        private Thickness chartThickness;

        private Border chartBorder;

        private bool isFirstChartLoad = true;

        private string chartUrl;

        private int singleTap = 0;

        private bool enlargeChart = false;

        private bool errorOpened;

        private bool _buttonChartCloseClicked = false;

        private bool _isFirstLoad = true;

        private Dictionary<String, String> test = new Dictionary<string, string>();

        private readonly Dictionary<Type, Action<BaseMessage>> messageHandlers = new Dictionary<Type, Action<BaseMessage>>();

        private readonly Dictionary<EventType, Func<string, BaseMessage>> jsonParser = new Dictionary<EventType, Func<string, BaseMessage>>();

        private List<KeyValuePair<string, object>> tagsList = new List<KeyValuePair<string, object>>();

        private DispatcherTimer timer;

        private DispatcherTimer loaderTimer;

        private string currentPage = "";

        private string preloadResponse;

        private bool _isLoggedIn;

        #endregion

        public MainPage()
        {
            this.mainUrl = "https://www.xforex.com/mobile-project/rc/wp8/engine_winrt.html"; //"https://test.xforex.com/mobile-project/test/wp8/engine_2048_new_login.html"; //
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            Application.Current.Suspending += OnSuspending;
            SetupMessageHandlers();
            GetPreloadData();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
             webView.InvokeScript("appSusspend", new[] {string.Empty});
        }

        private void App_Resuming(object sender, object e)
        {
             webView.InvokeScript("appResume", new[] {string.Empty});
        }

        private void SetupMessageHandlers()
        {
            this.jsonParser.Add(EventType.closeExternalView, JsonConvert.DeserializeObject<CloseExternalViewMessage>);
            this.jsonParser.Add(EventType.exit, JsonConvert.DeserializeObject<ExitMessage>);
            this.jsonParser.Add(EventType.hideExternalWebView, JsonConvert.DeserializeObject<HideExternalViewMessage>);
            this.jsonParser.Add(EventType.openChartWP8, JsonConvert.DeserializeObject<OpenChartMessage>);
            this.jsonParser.Add(EventType.chartInfo, JsonConvert.DeserializeObject<ChartInfoMessage>);
            this.jsonParser.Add(EventType.closeChartWP8, JsonConvert.DeserializeObject<CloseChartMessage>);
            this.jsonParser.Add(EventType.openExternalView, JsonConvert.DeserializeObject<OpenExternalViewMessage>);
            this.jsonParser.Add(EventType.openLink, JsonConvert.DeserializeObject<OpenUrlMessage>);
            this.jsonParser.Add(EventType.saveLoginPass, JsonConvert.DeserializeObject<UserCredentialsMessage>);
            this.jsonParser.Add(EventType.showExternalWebView, JsonConvert.DeserializeObject<ShowExternalWebViewMessage>);
            this.jsonParser.Add(EventType.trackEvent, JsonConvert.DeserializeObject<TrackEventMessage>);
            this.jsonParser.Add(EventType.goToPage, JsonConvert.DeserializeObject<GoToPageMessage>);
            this.jsonParser.Add(EventType.closeChart_BackBtn, JsonConvert.DeserializeObject<CloseChartBackBtnMessage>);
            this.jsonParser.Add(EventType.saveStorage, JsonConvert.DeserializeObject<SaveStorageMessage>);
            this.jsonParser.Add(EventType.error, JsonConvert.DeserializeObject<ErrorMessage>);

            this.messageHandlers.Add(typeof(ChartInfoMessage), message => ShowChart((message as ChartInfoMessage).Data));
            this.messageHandlers.Add(typeof(CloseChartMessage), message => CloseChart(message as CloseChartMessage));
            this.messageHandlers.Add(typeof(CloseExternalViewMessage), message =>
            {
                this.externalWebView.Visibility = Visibility.Collapsed;
            });
            //this.messageHandlers.Add(typeof(ExitMessage), message => Application.Current.Terminate());
            this.messageHandlers.Add(typeof(HideExternalViewMessage), message =>
            {
                this.externalWebView.Visibility = Visibility.Collapsed;
            });

            this.messageHandlers.Add(typeof(OpenChartMessage), message =>
            {
                this.chartUrl = (message as OpenChartMessage).Data.Url;
                this.ShowChart((message as OpenChartMessage).Data);
            });
            this.messageHandlers.Add(typeof(OpenExternalViewMessage), message => this.OpenExternalView(message as OpenExternalViewMessage));
            this.messageHandlers.Add(typeof(OpenUrlMessage), message => this.OpenUrl(message as OpenUrlMessage));
            this.messageHandlers.Add(typeof(GoToPageMessage), message => this.SetCurrentPage(message as GoToPageMessage));
            this.messageHandlers.Add(typeof(CloseChartBackBtnMessage), message => this.CloseChartBtmClicked(message as CloseChartBackBtnMessage));
            this.messageHandlers.Add(typeof(ShowExternalWebViewMessage), message =>
            {
               /* if (this.externalViewWebBrowser != null)
                {
                    this.externalViewWebBrowser.Visibility = Visibility.Visible;
                }*/
            });
            this.messageHandlers.Add(typeof(TrackEventMessage), message => this.TrackEvent(message as TrackEventMessage));
            this.messageHandlers.Add(typeof(UserCredentialsMessage), message => this.SaveUserData(message as UserCredentialsMessage));
            this.messageHandlers.Add(typeof(SaveStorageMessage), message => this.SaveAppDataToStorage(message as SaveStorageMessage));
            this.messageHandlers.Add(typeof(ErrorMessage), message =>
            {
                string s = (message as ErrorMessage).Data;
                if (s.Length > 0)
                {
                    int i = 0;
                    i++;
                }
            });
        }

        private void SaveAppDataToStorage(SaveStorageMessage message)
        {
            test = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingsHelper.DataJson);

            if (test != null)
            {
                string key = message._key;
                var val = message._obj.ToString();

                // 1 - уже есть ключ, проверка значения
                if (test.ContainsKey(key))
                {
                    string value = "";
                    if (test.TryGetValue(key, out value))
                    {
                        if (value != val)
                        {
                            test[key] = val;
                        }
                    }
                }
                else
                {
                    // добавить ключ / значение
                    test.Add(key, val);
                }

                var result = JsonConvert.SerializeObject(test);

                SettingsHelper.DataJson = result;
                webView.InvokeScript("setWP8Storage", new[] {result});
            }
            else
            {
                string key = message._key;
                var val = message._obj.ToString();
                if (key.Contains("language"))
                {
                    test = new Dictionary<string, string>();
                    test.Add(key, val);
                    var result = JsonConvert.SerializeObject(test);
                    SettingsHelper.DataJson = result;
                    webView.InvokeScript("setWP8Storage", new[] { result });
                }
            }
        }

        private void SaveUserProfile(string login, string password)
        {
            SettingsHelper.Login = login;
            SettingsHelper.Password = password;
        }

        private void SaveUserData(UserCredentialsMessage message)
        {
            if (message.Data.Login != null && message.Data.Pass != null)
            {
                if (SettingsHelper.Login == "" && SettingsHelper.Password == "")
                {
                    SaveUserProfile(message.Data.Login, message.Data.Pass);
                }
                else if (SettingsHelper.Login != message.Data.Login || SettingsHelper.Password != message.Data.Pass)
                {
                    SaveUserProfile(message.Data.Login, message.Data.Pass);
                }
            }
        }

        private void TrackEvent(TrackEventMessage message)
        {
           /* List<FlurryWP8SDK.Models.Parameter> parametres = new List<FlurryWP8SDK.Models.Parameter>();
            if (message.Data.EventName != null)
            {
                if (message.Data.EventValue != null)
                {
                    parametres.Add(new FlurryWP8SDK.Models.Parameter(message.Data.EventName, message.Data.EventValue));
                    FlurryWP8SDK.Api.LogEvent(message.Data.EventName, parametres);
                }
                else
                {
                    FlurryWP8SDK.Api.LogEvent(message.Data.EventName);
                }
            }*/
        }

        private void CloseChartBtmClicked(CloseChartBackBtnMessage closeChartBackBtnMessage)
        {
           /* this.block = true;
            await Chart.ClearInternetCacheAsync();
            Chart.Visibility = Visibility.Collapsed;
            Chart.NavigateToString("");*/
        }

        private void SetCurrentPage(GoToPageMessage goToPageMessage)
        {
            //throw new NotImplementedException();
        }

        private void OpenUrl(OpenUrlMessage message)
        {
            String url = message.Data.Url;
            int mail = String.Compare(url, 0, "mailto:", 0, 6);//url.compare(0, 7, "mailto:");
            int link = String.Compare(url, 0, "http", 0, 3);//4
            Launcher.LaunchUriAsync(new Uri(message.Data.Url));
            /*  if (link == 0)
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
                webBrowserTask.Show();
            }
            else if (mail == 0)
            {
                url = url.Substring(7);
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.Subject = "message subject";
                emailComposeTask.Body = "message body";
                emailComposeTask.To = url;
                emailComposeTask.Show();
            }
            else
            {
                url = GetServerUrl();
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
                webBrowserTask.Show();
            }*/
        }

        private void OpenExternalView(OpenExternalViewMessage openExternalViewMessage)
        {
            this.externalWebView.Margin = new Thickness(0, Convert.ToDouble(openExternalViewMessage.Data.Top), 0, 0);
            this.externalWebView.Navigate(new Uri(openExternalViewMessage.Data.Url));
            this.externalWebView.Visibility = Visibility.Visible;
        }

        private void CloseChart(CloseChartMessage closeChartMessage)
        {
            //throw new NotImplementedException();
        }

        private void ShowChart(Messages.MessageData.ChartInfo chartInfo)
        {
          //  this.chartWebView.Visibility = Visibility.Visible;;
           // this.chartWebView.Navigate(new Uri(this.chartUrl));

            //throw new NotImplementedException();
        }

        private void WebView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            int i = 0;
            i++;
        }

        private void WebView_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadStartPage();
        }

        private string GetServerUrl()
        {
            return "https://test.xforex.com";
        }
        private void LoadStartPage()
        {
            webView.Navigate(new Uri(mainUrl));
        }
        private void GetDeviceId()
        {
          /*  var deviceArrayID = (Byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            deviceID = Convert.ToBase64String(deviceArrayID);*/
        }

        private async void GetPreloadData()
        {
            string servUrl = GetServerUrl();
            //dynamic data = deviceID.Length > 0 ? new { deviceID = deviceID.ToString() } : new Object();
            HttpClient client = new HttpClient();
            StringContent queryString = new StringContent("123");
            Uri requestStr = new Uri(servUrl + "/mw/app/pre-login");
            HttpResponseMessage response = await client.PostAsync(requestStr, queryString);
            preloadResponse = await response.Content.ReadAsStringAsync();
        }

        private void SendSystemParamsMessage()
        {
            //this.Chart.Visibility = Visibility.Collapsed;
            //this.Chart.NavigateToString("");
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingsHelper.DataJson);
            string savedLanguage = string.Empty;
            if (jsonData != null)
            {
                if (jsonData.TryGetValue("language", out savedLanguage))
                {
                    language = savedLanguage;
                }
            }
            
            var result = JsonConvert.SerializeObject(new SystemParamsMessage { DefaultLanguage = language, OsId = "os_id_windows", DeviceId = this.deviceID });
            webView.InvokeScript("documentReady_part2", new []{result});
        }

        private void SendInitializeDataToWebView()
        {
            webView.InvokeScript("updateNetworkState", new []{(NetworkInterface.GetIsNetworkAvailable() ? "true" : "false")});
            if (preloadResponse != null)
            {
                webView.InvokeScript("wp8SetappVersionResponse",
                    new[] { JsonConvert.SerializeObject(JsonConvert.DeserializeObject<PreloadResponse>(preloadResponse))});
            }

            if (SettingsHelper.Login != "" && SettingsHelper.Password != "")
            {
                webView.InvokeScript("wp8LoginPass", new[] { SettingsHelper.Login, SettingsHelper.Password });
            }

            webView.InvokeScript("sendAppVersionReqeust", new[] { string.Empty });
            webView.InvokeScript("sendAppVersionReqeust",  new[] {"DEVICE_ID='" + deviceID + "';"});

            if (autologin)
            {
                webView.InvokeScript("autologin", new[] { string.Empty });
                autologin = false;
            }

            //Splash.Visibility = Visibility.Collapsed;
           // webView.InvokeScript("setWidthToFields", new []{string.Empty});
        }

        private void Logout()
        {
            //Splash.Visibility = Visibility.Visible;
            _isLoggedIn = false;
            LoadStartPage();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            int i = 0;
            i++;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int i = 0;
            i++;
        }

        private void WebView_OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value == "documentLoaded")
            {
                SendSystemParamsMessage();
            }
            else if (e.Value == "loading_step")
            {
                //ChangeLoadingStep();
            }
            else if (e.Value == "showWebView")
            {
               SendInitializeDataToWebView();
               /* if (!autologin)
                {
                    myWB.Visibility = Visibility;
                }*/
            }
            else if (e.Value == "on_logout")
            {
               Logout();
            }
            else if (e.Value == "on_lang_change")
            {
                Logout();
                autologin = true;
            }
            else if (e.Value == "onShowChart")
            {
                // ShowChart();
            }
            else if (e.Value == "onHideChart")
            {
               // HideChart();
            }
            else if (e.Value == "clearCache")
            {
                //webView .ClearInternetCacheAsync();
            }
            else if (e.Value == "logined")
            {
                _isLoggedIn = true;
                if (autologin)
                {
                    autologin = false;
                }
            }
            else if (e.Value == "rateApp")
            {
                //MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                //marketplaceReviewTask.Show();
            }
            else
            {
                var message = this.ParseJson(e.Value);
                try
                {
                    this.messageHandlers[message.GetType()](message);
                }
                catch { }

            }
        }

        private BaseMessage ParseJson(string json)
        {
            BaseMessage message;
            try
            {
                message = JsonConvert.DeserializeObject<BaseMessage>(json);
            }
            catch
            {
                return null;
            }

            return this.jsonParser[message.Event](json);
        }

    }
}