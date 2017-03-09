
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PushSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Storage;
using Xtrade_wp8.Com;
using Xtrade_wp8.Com.Messages;
using Xtrade_wp8.Com.Messages.MessageData;
using Xtrade_wp8.Com.Messages.MessageTypes;
using Xtrade_wp8.Com.VO;
using Xtrade_wp8.Controls;
using Xtrade_wp8.Resources;
using System.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace Xtrade_wp8
{
    public partial class MainPage : INotifyPropertyChanged
    {
        private UserData userData = new UserData("", "");

        private bool autologin;

        private string deviceID;

        private string mainUrl;

        private WebBrowser externalViewWebBrowser;

        private ChartsData chartsData;

        private Thickness chartThickness;

        private Thickness apmPopupThickness;

        private Border chartBorder;

        private bool isFirstChartLoad = true;

        private string chartUrl;

        private int singleTap = 0;

        private bool enlargeChart = false;

        private bool errorOpened;

        private Popup popup = new Popup();

        private ErrorControl control;

        private bool _buttonChartCloseClicked = false;

        private bool _isFirstLoad = true;

        private Dictionary<String, String> test = new Dictionary<string, string>();

        private readonly Dictionary<Type, Action<BaseMessage>> messageHandlers = new Dictionary<Type, Action<BaseMessage>>();

        private readonly Dictionary<EventType, Func<string, BaseMessage>> jsonParser = new Dictionary<EventType, Func<string, BaseMessage>>();

        private List<KeyValuePair<string, object>> tagsList = new List<KeyValuePair<string, object>>();

        private DispatcherTimer timer;

        private DispatcherTimer timer1;

        private string currentPage = "";

        private string preloadResponse;

        private bool _isLoggedIn;

        private string apmDepositStatus;

        public Thickness ChartThickness
        {
            get { return this.chartThickness; }
            set
            {
                this.chartThickness = value;
                this.OnPropertyChanged();
            }
        }

        public Thickness PopupThickness
        {
            get { return this.apmPopupThickness; }
            set
            {
                this.apmPopupThickness = value;
                this.OnPropertyChanged();
            }
        }

        private bool isPhotoTask = false;

        private NotificationService service;

        public MainPage()
        {
            // jsonParser
            this.jsonParser.Add(EventType.getscript, JsonConvert.DeserializeObject<GetScriptData>);
            this.jsonParser.Add(EventType.closeExternalView, JsonConvert.DeserializeObject<CloseExternalViewMessage>);
            this.jsonParser.Add(EventType.exit, JsonConvert.DeserializeObject<ExitMessage>);
            this.jsonParser.Add(EventType.hideExternalWebView, JsonConvert.DeserializeObject<HideExternalViewMessage>);
            this.jsonParser.Add(EventType.openChartWP8, JsonConvert.DeserializeObject<OpenChartMessage>);
            this.jsonParser.Add(EventType.closeChartWP8, JsonConvert.DeserializeObject<CloseChartMessage>);
            this.jsonParser.Add(EventType.openExternalView, JsonConvert.DeserializeObject<OpenExternalViewMessage>);
            this.jsonParser.Add(EventType.openLink, JsonConvert.DeserializeObject<OpenUrlMessage>);
            this.jsonParser.Add(EventType.saveLoginPass, JsonConvert.DeserializeObject<UserCredentialsMessage>);
            this.jsonParser.Add(EventType.showExternalWebView, JsonConvert.DeserializeObject<ShowExternalWebViewMessage>);
            this.jsonParser.Add(EventType.trackEvent, JsonConvert.DeserializeObject<TrackEventMessage>);
            this.jsonParser.Add(EventType.goToPage, JsonConvert.DeserializeObject<GoToPageMessage>);
            this.jsonParser.Add(EventType.closeChart_BackBtn, JsonConvert.DeserializeObject<closeChartBackBtnMessage>);
            this.jsonParser.Add(EventType.saveStorage, JsonConvert.DeserializeObject<SaveStorageMessage>);
            this.jsonParser.Add(EventType.openApmPopup, JsonConvert.DeserializeObject<OpenApmPopupMessage>);
            this.jsonParser.Add(EventType.sendDoc, JsonConvert.DeserializeObject<SendDocMessage>);
            this.jsonParser.Add(EventType.apmDeposit, JsonConvert.DeserializeObject<ApmDepositMessage>);
            this.jsonParser.Add(EventType.wp8_restartApp, JsonConvert.DeserializeObject<wp8_restartApp>);

            // messageHandlers
            this.messageHandlers.Add(typeof(CloseChartMessage), message => CloseChart(message as CloseChartMessage));
            this.messageHandlers.Add(typeof(CloseExternalViewMessage), message => this.RemoveExternalWebView(this.externalViewWebBrowser));
            this.messageHandlers.Add(typeof(ExitMessage), message => Application.Current.Terminate());
            this.messageHandlers.Add(typeof(TrackEventMessage), message => this.TrackEvent(message as TrackEventMessage));
            this.messageHandlers.Add(typeof(UserCredentialsMessage), message => this.SaveUserData(message as UserCredentialsMessage));
            this.messageHandlers.Add(typeof(SaveStorageMessage), message => this.SaveAppDataToStorage(message as SaveStorageMessage));
            this.messageHandlers.Add(typeof(OpenApmPopupMessage), message => this.OpenApmPopup(message as OpenApmPopupMessage));
            this.messageHandlers.Add(typeof(SendDocMessage), message => this.SendDocument(message as SendDocMessage));
            this.messageHandlers.Add(typeof(OpenExternalViewMessage), message => this.OpenExternalView(message as OpenExternalViewMessage));
            this.messageHandlers.Add(typeof(OpenUrlMessage), message => this.OpenUrl(message as OpenUrlMessage));
            this.messageHandlers.Add(typeof(GetScriptData), message => this.GetScript(message as GetScriptData));
            this.messageHandlers.Add(typeof(GoToPageMessage), message => this.setCurrentPage(message as GoToPageMessage));
            this.messageHandlers.Add(typeof(wp8_restartApp), message => this.ReloadApplicatiom(message as wp8_restartApp));
            this.messageHandlers.Add(typeof(closeChartBackBtnMessage), message => this.closeChartBtmClicked(message as closeChartBackBtnMessage));

            this.messageHandlers.Add(typeof(ApmDepositMessage), message =>
            {
                ApmPopup.Visibility = Visibility.Collapsed;
                myWB.InvokeScript("_CFullDeposit_fn_onDepositAPMPopupResult", (message as ApmDepositMessage).apmDepositStatus);
            });

            this.messageHandlers.Add(typeof(HideExternalViewMessage), message =>
            {
                if (ApmPopup.Visibility == Visibility.Visible)
                {
                    ApmPopup.Visibility = Visibility.Collapsed;
                }

                if (this.externalViewWebBrowser != null)
                {
                    this.externalViewWebBrowser.Visibility = Visibility.Collapsed;
                }
            });

            this.messageHandlers.Add(typeof(OpenChartMessage), message =>
            {
                this.chartUrl = (message as OpenChartMessage).Data.Url;
                this.ShowChart((message as OpenChartMessage).Data);
            });


            this.messageHandlers.Add(typeof(ShowExternalWebViewMessage), message =>
            {
                if (this.externalViewWebBrowser != null)
                {
                    this.externalViewWebBrowser.Visibility = Visibility.Visible;
                }
            });

            InitializeComponent();
            InitSplashScreen();
            DataContext = this;
            InitializeTrackerPushWoosh();
            Initialize();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += PhotoChooserTask_Completed;
        }

        //protected override void OnBackKeyPress(CancelEventArgs e)
        //{
        //    // put any code you like here
        //    MessageBox.Show("You pressed the Back button");
        //    e.Cancel = true;
        //}

        private void PhotoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                UploadPhoto(e);
            }
        }


        private OpenApmPopupMessage popupMessage;

        private readonly PhotoChooserTask photoChooserTask;

        private SendDocMessage sendDocMessage;

        private string pushToken;

        private void SendDocument(SendDocMessage message)
        {
            sendDocMessage = message;
            isPhotoTask = true;
            photoChooserTask.Show();
        }

        public void ReloadApplicatiom(wp8_restartApp data)
        {
            statPageLoaded = true;
            myWB.Base = "wp8";
            myWB.Navigate(new Uri("engine_wp8_nativ.html", UriKind.Relative));
        }

        private void OpenApmPopup(OpenApmPopupMessage message)
        {
            popupMessage = message;
            PopupThickness = new Thickness(0, message.Top, 0, 0);
            ApmPopup.Height = message.Height;
            ApmPopup.Width = message.Width;
            ApmPopup.Navigate(new Uri(Constants.APM_POPUP_URL));
        }

        private void SendSystemParamsMessage()
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingsHelper.DataJson);
            string savedLanguage = string.Empty;

            if (jsonData != null)
            {
                myWB.InvokeScript("setWP8Storage", SettingsHelper.DataJson);

                if (jsonData.TryGetValue("language", out savedLanguage))
                {
                    language = savedLanguage;
                }
            }

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();

            var result = JsonConvert.SerializeObject(
                new SystemParamsMessage
                {
                    DefaultLanguage = language,
                    OsId = "os_id_windows",
                    DeviceId = this.deviceID,
                    pushwooshToken = service.PushToken,
                    localStorage = jsonData
                }).Replace("False", "false").Replace("True", "true");

            myWB.InvokeScript("documentReady_part2", result);
        }

        private void ChangeLoadingStep()
        {
            Loader.Value += 20;

            if (Loader.Value >= 100)
            {
                Loader.Visibility = Visibility.Collapsed;
            }
        }

        private void Initialize()
        {
            GetDeviceId();
            GetWinVersion();
            GetSavedUserData();
            chartsData = new ChartsData();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(Constants.URL_UPLOAD_MAX_TIME);
            timer.Tick += TimerOnTick;
            timer.Start();

            timer1 = new DispatcherTimer();

            if (userData.Login != "" && userData.Password != "")
            {
                timer1.Interval = TimeSpan.FromSeconds(7);
            }
            else
            {
                timer1.Interval = TimeSpan.FromSeconds(4);
            }

            timer1.Tick += TimerOnTick1;

            getPreloadData();
        }

        private async void getPreloadData()
        {
            string servUrl = GetServerUrl();

            try
            {
                HttpClient client = new HttpClient();
                StringContent queryString = new StringContent(deviceID);
                Uri requestStr = new Uri(servUrl + "/mw/app/pre-login");
                HttpResponseMessage response = await client.PostAsync(requestStr, queryString);
                preloadResponse = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                int i = 0;
                i++;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!isPhotoTask)
            {
                string version = Environment.OSVersion.ToString();

                if (version.Contains(Constants.WP_8_0))
                {
                    errorOpened = true;
                    var result = MessageBox.Show(AppResources.ErrorText);

                    if (result == MessageBoxResult.OK)
                    {
                        Application.Current.Terminate();
                    }
                }

                if (!_isFirstLoad)
                {
                    SupportedOrientations = SupportedPageOrientation.Portrait;
                    Splash.Visibility = Visibility.Visible;
                    Loader.Visibility = Visibility.Visible;
                    Loader.Value = 0;
                    autologin = _isLoggedIn;
                    myWB.ClearInternetCacheAsync();
                    LoadStartPage();
                }
            }

            isPhotoTask = false;
        }

        private async void closeChartBtmClicked(closeChartBackBtnMessage message)
        {

        }

        private void CloseChart(CloseChartMessage data)
        {

        }

        private void setCurrentPage(GoToPageMessage message)
        {
            currentPage = message.Data as String;
        }

        private void InitializeTrackerPushWoosh()
        {
            service = NotificationService.GetCurrent(Constants.PUSHWOOSH_KEY, null, null);

            service.OnPushTokenReceived += (sender, args) =>
            {
                pushToken = args.ToString();
                //MessageBox.Show("pushToken: " + pushToken);
            };

            service.OnPushTokenFailed += (sender, args) =>
            {
            };

            service.OnPushAccepted += (sender, args) =>
            {
                MessageBox.Show(args.Content.ToString());
            };

            service.SubscribeToPushService();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                if (control == null) { control = new ErrorControl(); }

                popup.Child = control;
                popup.IsOpen = true;

                control.ErrorText = AppResources.ErrorPopupText;
                control.buttonOk.Click += (s, args) =>
                {
                    popup.IsOpen = false;
                    popup.Child = null;
                };
            }
        }

        private void TimerOnTick1(object sender, EventArgs eventArgs)
        {
            Splash.Visibility = Visibility.Collapsed;
            Loader.Visibility = Visibility.Collapsed;
            timer1.Stop();
        }

        private void GetWinVersion()
        {
            string version = Environment.OSVersion.ToString();

            if (version.Contains(Constants.WP_8_0))
            {
                errorOpened = true;
                var result = MessageBox.Show(AppResources.ErrorText);

                if (result == MessageBoxResult.OK)
                {
                    Application.Current.Terminate();
                }

                mainUrl = Constants.MAIN_URL_WP8_0;
            }
            else
            {
                mainUrl = Constants.MAIN_URL_WP8_1;
            }
        }

        private void InitSplashScreen()
        {
            switch (ResolutionHelper.CurrentResolution)
            {
                case Resolutions.HD:
                    {
                        Splash.Source = new BitmapImage(new Uri("SplashScreenImage.screen-720p.png", UriKind.Relative));
                        break;
                    }
                case Resolutions.WXGA:
                    {
                        Splash.Source = new BitmapImage(new Uri("SplashScreenImage.screen-WVGA.png", UriKind.Relative));
                        break;
                    }
                case Resolutions.WVGA:
                    {
                        Splash.Source = new BitmapImage(new Uri("SplashScreenImage.screen-WXGA.png", UriKind.Relative));
                        break;
                    }
            }

            Loader.Visibility = Visibility.Visible;
        }
        private void LoadStartPage()
        {
            LoaderProgress();

            if (errorOpened)
                Application.Current.Terminate();

            CheckZipVersion();
        }

        private bool statPageLoaded;

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {

            if (statPageLoaded)
            {
                timer1.Start();
                statPageLoaded = false;
            }

            myWB.Visibility = Visibility.Visible;
            _isFirstLoad = false;
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            myWB.Visibility = Visibility.Collapsed;
            LoadStartPage();
        }

        bool isdocumentLoadedEnd = true;

        private void GetScript(GetScriptData name)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string fileName = name.data;

                if (!file.FileExists(fileName))
                {
                    return;
                }

                var isoStream = file.OpenFile(fileName, FileMode.Open);

                using (StreamReader reader = new StreamReader(isoStream))
                {
                    string html = reader.ReadToEnd();

                    myWB.InvokeScript("WP8_AddScript", html);
                }
            }
        }

        private void myWB_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Logger.Show(DateTime.Now.ToLongTimeString() + " | START |  " + e.Value);

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string mystring = localFolder.Path;

            if (e.Value == "documentLoaded")
            {
                SendSystemParamsMessage();
            }
            else if (e.Value == "loading_step")
            {
                ChangeLoadingStep();
            }
            else if (e.Value == "showWebView")
            {
                myWB.Visibility = Visibility.Collapsed;
                SendInitializeDataToWebView();

                if (!autologin)
                {
                    myWB.Visibility = Visibility;
                }
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
                ShowChart();
            }
            else if (e.Value == "onHideChart")
            {
                HideChart();
            }
            else if (e.Value == "clearCache")
            {
                myWB.ClearInternetCacheAsync();
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
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
            }
            else if (e.Value == "success_close")
            {
                // [s appendString:@"(true);"];
                CloseApmPopup();
                myWB.InvokeScript(popupMessage.Callback, "(true);");
            }
            else if (e.Value == "fail_close")
            {
                // [s appendString:@"(false);"];
                CloseApmPopup();
                myWB.InvokeScript(popupMessage.Callback, "(false);");
            }
            else if (e.Value == "end")
            {
                // [s appendString:@"(true);"];
                CloseApmPopup();
                myWB.InvokeScript(popupMessage.Callback, "(true);");
            }
            else if (e.Value == "fillFormBY")
            {
                // [s appendString:@"(false);"];
                CloseApmPopup();
                myWB.InvokeScript(popupMessage.Callback, "(false);");
            }
            else
            {
                var message = this.ParseJson(e.Value);

                try
                {

                    this.messageHandlers[message.GetType()](message);
                }

                catch (Exception ex)
                {

                }
            }

            Logger.Show(DateTime.Now.ToLongTimeString() + " | END  |  " + e.Value);
        }

        private async void CloseApmPopup()
        {
            await ApmPopup.ClearInternetCacheAsync();
            ApmPopup.Visibility = Visibility.Collapsed;
            ApmPopup.NavigateToString("");
        }

        private BaseMessage ParseJson(string json)
        {
            BaseMessage message;
            try
            {
                message = JsonConvert.DeserializeObject<BaseMessage>(json);
            }
            catch (Exception ex)
            {
                return null;
            }

            return this.jsonParser[message.Event](json);
        }

        private void SaveUserData(UserCredentialsMessage message)
        {
            if (message.Data.Login != null && message.Data.Pass != null)
            {
                if (userData.Login == "" && userData.Password == "")
                {
                    userData = new UserData(message.Data.Login, message.Data.Pass);
                    SaveUserProfile();
                }
                else if (userData.Login != message.Data.Login || userData.Password != message.Data.Pass)
                {
                    userData = new UserData(message.Data.Login, message.Data.Pass);
                    SaveUserProfile();
                }
            }
        }

        private void SaveAppDataToStorage(SaveStorageMessage message)
        {
            test = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingsHelper.DataJson);

            if (test != null)
            {
                string key = message._key.ToString();
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
                myWB.InvokeScript("setWP8Storage", result);
            }
            else
            {
                string key = message._key.ToString();
                var val = message._obj.ToString();
                test = new Dictionary<string, string>();
                test.Add(key, val);
                var result = JsonConvert.SerializeObject(test);
                SettingsHelper.DataJson = result;
                myWB.InvokeScript("setWP8Storage", result);
            }
        }

        private void TrackEvent(TrackEventMessage message)
        {
            List<FlurryWP8SDK.Models.Parameter> parametres = new List<FlurryWP8SDK.Models.Parameter>();

            string eventName = message.Data.EventName;
            string eventVal = message.Data.EventValue;

            if (eventName != null)
            {
                Dictionary<string, Object> addToCartParams = new Dictionary<string, Object>();

                if (eventVal != null)
                {
                    //parametres.Add(new FlurryWP8SDK.Models.Parameter(message.Data.EventName, message.Data.EventValue));
                    //FlurryWP8SDK.Api.LogEvent(message.Data.EventName, parametres);
                    addToCartParams.Add(eventName, "");
                }
                else
                {
                    addToCartParams.Add(eventName, eventVal);
                    //FlurryWP8SDK.Api.LogEvent(message.Data.EventName);
                }

                AppsFlyerLib.AppsFlyerTracker.GetAppsFlyerTracker().TrackEvent(eventName, addToCartParams);
            }
        }

        private void OpenUrl(OpenUrlMessage message)
        {
            string url = message.Data.Url;
            int mail = string.Compare(url, 0, "mailto:", 0, 6);//url.compare(0, 7, "mailto:");
            int link = string.Compare(url, 0, "http", 0, 3);//4

            if (link == 0)
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
            }
        }

        private void OpenExternalView(OpenExternalViewMessage message)
        {
            externalViewWebBrowser = new WebBrowser();
            externalViewWebBrowser.IsScriptEnabled = true;
            ContentPanel.Children.Add(externalViewWebBrowser);
            externalViewWebBrowser.Margin = new  Thickness(0,65,0,0);
            externalViewWebBrowser.Navigate(new Uri(message.Data.Url));
        }

        private void RemoveExternalWebView(WebBrowser webBrowser)
        {
            if (webBrowser != null)
            {
                ContentPanel.Children.Remove(webBrowser);
            }
        }

        private string GetServerUrl()
        {
            string address = Constants.IS_DEBUG ? Constants.SERVER_ADDRESS_TEST_PREF : Constants.SERVER_ADDRESS_DEV_PREF;
            // string address = Constants.SERVER_ADDRESS_DEV_PREF;
            string servUrl = string.Format("{0}{1}.{2}", Constants.SERVER_ADDRESS_PROTOCOL, address, Constants.SERVER_ADDRESS);
            return servUrl;
        }

        private void ShowChart(ChartInfo data)
        {

        }

        private async void ShowSmallChat()
        {

        }

        private void ShowEnlargedChat()
        {

        }

        private void CreateFile(BinaryWriter bw, byte[] datatest)
        {

        }

        private void Logout()
        {
            Splash.Visibility = Visibility.Visible;
            _isLoggedIn = false;
            LoadStartPage();
        }

        private void ShowChart()
        {
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }

        private void HideChart()
        {
            SupportedOrientations = SupportedPageOrientation.Portrait;
        }

        private void ExternalBrowserHide()
        {
            if (externalViewWebBrowser != null) { externalViewWebBrowser.Visibility = Visibility.Collapsed; }
        }

        private void GetSavedUserData()
        {
            userData = new UserData(SettingsHelper.Login, SettingsHelper.Password);
        }

        private void SaveUserProfile()
        {
            SettingsHelper.Login = userData.Login;
            SettingsHelper.Password = userData.Password;
        }

        private void SendInitializeDataToWebView()
        {
            Logger.Show("sendInitializeDataToWebView->begin ");

            myWB.InvokeScript("updateNetworkState", (NetworkInterface.GetIsNetworkAvailable() ? "true" : "false"));

            if (preloadResponse != null)
            {
                var deser = JsonConvert.DeserializeObject<PreloadResponse>(preloadResponse);
                var ser = JsonConvert.SerializeObject(deser);
                myWB.InvokeScript("wp8SetappVersionResponse", ser);
            }

            myWB.InvokeScript("wp8LoginPass", SettingsHelper.Login, SettingsHelper.Password);

            myWB.InvokeScript("sendAppVersionReqeust");
            myWB.InvokeScript("sendAppVersionReqeust", "DEVICE_ID='" + deviceID + "';" + "PUSH_TOKEN='" + service.PushToken + "';");

            if (autologin)
            {
                myWB.InvokeScript("autologin", "");
                autologin = false;
            }
        }

        private void GetDeviceId()
        {
            var deviceArrayID = (Byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            deviceID = Convert.ToBase64String(deviceArrayID);
            Logger.Show(deviceID);
        }

        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            myWB.InvokeScript("getChartInfoWP8");
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;

            if (ApmPopup.Visibility == Visibility.Collapsed)
            {
                enlargeChart = false;

                try
                {
                    myWB.InvokeScript("onAndroidSysBtnPressed", "KEY_BACK");
                }
                catch
                {
                    Logger.Show("OnBackKeyPress JS script call error");
                }
            }
            else
            {
                ApmPopup.Visibility = Visibility.Collapsed;
                // myWB.InvokeScript(popupMessage.Callback, "failed");
                myWB.InvokeScript("onAndroidSysBtnPressed", "KEY_BACK");
            }
        }

        private void Border_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            e.Handled = true;
        }

        private void Border_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // suppress zoom
            /* if (e.DeltaManipulation.Scale.X != 0.0 ||
                 e.DeltaManipulation.Scale.Y != 0.0)*/
            e.Handled = true;

            // optionally suppress scrolling

            /*  if (e.DeltaManipulation.Translation.X != 0.0 ||
                  e.DeltaManipulation.Translation.Y != 0.0)*/
            e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        private void Chart_OnLoadCompleted(object sender, NavigationEventArgs e)
        {

        }

        private async void Chart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ApmPopup_OnLoaded(object sender, RoutedEventArgs e)
        {
            ApmPopup.Visibility = Visibility.Collapsed;
        }

        private void ApmPopup_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var message = this.ParseJson(e.Value);

            try
            {
                this.messageHandlers[message.GetType()](message);
            }

            catch (Exception ex)
            {

            }
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }

        string checkDoubleRequestPopup = "";

        private void ApmPopup_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new LowercaseContractResolver();
                var dataForJS = JsonConvert.SerializeObject(popupMessage.Data, Formatting.Indented, settings).Replace("\r\n", "");

                if (checkDoubleRequestPopup != dataForJS)
                {
                    ApmPopup.Visibility = Visibility.Visible;
                    checkDoubleRequestPopup = dataForJS;
                    ApmPopup.InvokeScript("fillFormBY", dataForJS);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void UploadPhoto(PhotoResult chosenPhoto)
        {
            myWB.InvokeScript("wp8_startLoadDocument", "");

            BitmapSource bitmapSource = new BitmapImage();
            bitmapSource.SetSource(chosenPhoto.ChosenPhoto);
            var writeableBitmap = new WriteableBitmap(bitmapSource);

            var memoryStream = new MemoryStream();
            writeableBitmap.SaveJpeg(memoryStream, 200, 200, 0, 70);
            byte[] avatar = memoryStream.ToArray();
            memoryStream.Close();

            var url = string.Format("{0}{1}?mobile_phone=true&sessionToken={2}", "https://trade.xtrade.com/mw-secured/uploadFile/",
                sendDocMessage.Data.Suffix.Substring(0, sendDocMessage.Data.Suffix.Length - 1),
                sendDocMessage.Data.SessionToken);

            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    HttpContent bytesContent = new ByteArrayContent(avatar);

                    string fileName = chosenPhoto.OriginalFileName.Split('\\').Last();

                    bytesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = sendDocMessage.Data.Field,
                        FileName = fileName
                    };

                    string fileNameType = fileName.Split('.').Last().ToLower();

                    switch (fileNameType)
                    {
                        case "png":
                            fileNameType = "image/png";
                            break;
                        case "tiff":
                            fileNameType = "image/tiff";
                            break;
                        case "gif":
                            fileNameType = "image/gif";
                            break;
                        case "jpg":
                            fileNameType = "image/jpeg";
                            break;
                        case "jpeg":
                            fileNameType = "image/jpeg";
                            break;
                    }

                    bytesContent.Headers.ContentType = new MediaTypeHeaderValue(fileNameType);

                    formData.Add(bytesContent);
                    var response = await client.PostAsync(url, formData);
                    var result = (await response.Content.ReadAsStringAsync());

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        myWB.InvokeScript("wp8_completedLoadDocument", sendDocMessage.Data.foobar);
                    }
                    else
                    {
                        myWB.InvokeScript("wp8_failLoadDocument", "");
                    }
                }
            }
        }

        private void CheckZipVersion()
        {
            LoaderProgress();
            var obj = App.Current as App;

            if (!obj.IsActivated)
            {
                try
                {
                    //Create a webclient that'll handle your download
                    WebClient client = new WebClient();
                    //Run function when resource-read (OpenRead) operation is completed
                    client.OpenReadCompleted += client_OpenVersionReadCompleted;
                    //Start download / open stream
                    client.OpenReadAsync(new Uri("http://test.xforex.com/mobile-project/test/wp8/version.txt"));//Use the url to your file instead, this is just a test file (pdf)               
                }
                catch (Exception ex)
                {
                    MessageBox.Show("We could not start your download. Error message: " + ex.Message);
                }
            }
            else
            {
                statPageLoaded = true;
                myWB.Base = "wp8";
                myWB.Navigate(new Uri("engine_wp8_nativ.html", UriKind.Relative));
            }
        }

        private string GetFileText(string fileName)
        {
            string text = "";

            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!file.FileExists(fileName))
                {
                    return text;
                }

                var isoStream = file.OpenFile(fileName, FileMode.Open);

                using (StreamReader reader = new StreamReader(isoStream))
                {
                    text = reader.ReadToEnd();
                }
            }

            return text;
        }

        async private void client_OpenVersionReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            LoaderProgress();

            string fileName = "version.txt";
            byte[] buffer = null;
            try
            {
                buffer = new byte[e.Result.Length];
            }
            catch (Exception ex)
            {
                var obj = App.Current as App;

                obj.CointerErrorDownload++;

                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    file.DeleteFile("version.txt");
                }
                if (!(obj.CointerErrorDownload > 5))
                {
                    LoadStartPage(); return;
                }
                else
                {
                    Application.Current.Terminate();
                }
            }
            //Store bytes in buffer, so it can be saved later on
            await e.Result.ReadAsync(buffer, 0, buffer.Length);

            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Create file
                using (IsolatedStorageFileStream stream = file.OpenFile(fileName, FileMode.Create))
                {
                    //Write content stream to file
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }

            LoadOrDownload();
        }


        async private void client_OpenVersionReadCompletedHash(object sender, OpenReadCompletedEventArgs e)
        {
            LoaderProgress();

            string fileName = "hash.txt";
            byte[] buffer = null;
            try
            {
                buffer = new byte[e.Result.Length];
            }
            catch (Exception ex)
            {

                var obj = App.Current as App;

                obj.CointerErrorDownload++;

                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    file.DeleteFile("version.txt");
                }
                if (!(obj.CointerErrorDownload > 5))
                {
                    LoadStartPage(); return;
                }
                else
                {
                    Application.Current.Terminate();
                }
            }
            //Store bytes in buffer, so it can be saved later on
            await e.Result.ReadAsync(buffer, 0, buffer.Length);

            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Create file
                using (IsolatedStorageFileStream stream = file.OpenFile(fileName, FileMode.Create))
                {
                    //Write content stream to file
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }

        private void LoadOrDownload()
        {
            LoaderProgress();

            string fileName = "version.txt";

            string version_Server1 = GetFileText(fileName).Trim();
            string version_Iso1 = GetFileText("\\wp8\\" + fileName).Trim();

            if (version_Server1 == version_Iso1 && version_Server1 != "")
            {
                try
                {
                    //Create a webclient that'll handle your download
                    WebClient client = new WebClient();
                    //Run function when resource-read (OpenRead) operation is completed
                    client.OpenReadCompleted += client_OpenVersionReadCompletedHash;
                    //Start download / open stream
                    client.OpenReadAsync(new Uri("https://www.xforex.com/mobile-project/rc/wp8/hash.txt"));//Use the url to your file instead, this is just a test file (pdf)               
                }
                catch (Exception ex)
                {
                    MessageBox.Show("We could not start your download. Error message: " + ex.Message);
                }


                //string fileNameHash = "hash.txt";

                //string version_Server1Hash = GetFileText(fileNameHash).Trim();
                //string version_Iso1Hash = GetFileText("\\wp8\\" + fileNameHash).Trim();

                //if (version_Server1Hash == version_Iso1Hash && version_Server1Hash != "")
                //{

                LoaderProgress();



                string version_engine = GetFileText("\\wp8\\engine_wp8_nativ.html").Trim();

                if (version_engine != "")
                {
                    statPageLoaded = true;
                    myWB.Base = "wp8";
                    myWB.Navigate(new Uri("engine_wp8_nativ.html", UriKind.Relative));
                }

                //}
                else
                {
                    LoadJsZip();
                }
            }
            else
            {
                LoadJsZip();
            }
        }

        private void LoadJsZip()
        {
            LoaderProgress();

            try
            {
                //Create a webclient that'll handle your download
                WebClient client = new WebClient();
                //Run function when resource-read (OpenRead) operation is completed
                client.OpenReadCompleted += client_OpenReadCompleted;
                //Start download / open stream
                client.OpenReadAsync(new Uri("http://test.xforex.com/mobile-project/test/wp8/wp8.zip"));//Use the url to your file instead, this is just a test file (pdf)
            }
            catch (Exception ex)
            {
                MessageBox.Show("We could not start your download. Error message: " + ex.Message);
            }
        }

        private int tryDownloadZip = 0;

        public static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }


        async void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            LoaderProgress();

            bool isLoadZip = true;

            if (isLoadZip)
            {
                try
                {
                    byte[] buffer = null;
                    try
                    {
                        buffer = new byte[e.Result.Length];
                    }
                    catch (Exception ex)
                    {
                        var obj = App.Current as App;

                        obj.CointerErrorDownload++;

                        using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            file.DeleteFile("version.txt");
                        }
                        if (!(obj.CointerErrorDownload > 5))
                        {
                            LoadStartPage(); return;

                        }
                        else
                        {
                            Application.Current.Terminate();
                        }
                    }
                    //Store bytes in buffer, so it can be saved later on
                    await e.Result.ReadAsync(buffer, 0, buffer.Length);
                    //Store to isolatedstorage
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //Create file
                        using (IsolatedStorageFileStream stream = file.OpenFile("wp8.zip", FileMode.Create))
                        {
                            //Write content stream to file
                            await stream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tryDownloadZip++;
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        file.DeleteDirectory("wp8");
                    }

                    if (tryDownloadZip > 2)
                    {
                        MessageBox.Show("We could not finish your download. Error message: " + ex.Message);
                    }
                    else
                    {
                        LoadJsZip();
                    }
                }
            }

            Thread thread = new Thread(ExtractZip);
            thread.Start();
        }



        private void LoaderProgress(int count = 5)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Loader.Value += count;
            });
        }

        private void ExtractZip()
        {
            LoaderProgress();

            int countZip = 0;

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                LoaderProgress();

                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("wp8.zip", FileMode.Open, FileAccess.ReadWrite))
                {
                    LoaderProgress();

                    UnZipper unzip = new UnZipper(fileStream);

                    foreach (string filename in unzip.FileNamesInZip())
                    {
                        Stream fileFromZip = unzip.GetFileStream(filename);

                        if (filename.Contains("/."))
                        {
                            continue;
                        }

                        var dicName = Path.GetDirectoryName(filename);

                        if (!myIsolatedStorage.DirectoryExists(dicName))
                        {
                            myIsolatedStorage.CreateDirectory(dicName);
                        }

                        StorageFolder localFolder1 = ApplicationData.Current.LocalFolder;
                        string mystring1 = localFolder1.Path;

                        if (fileFromZip != null)
                        {
                            using (var fileStream1 = new FileStream(mystring1 + "\\" + filename, FileMode.Create, FileAccess.Write))
                            {
                                fileFromZip.CopyTo(fileStream1);
                            }

                            byte[] buffer1 = new byte[fileFromZip.Length];
                            //Store bytes in buffer, so it can be saved later on
                            fileFromZip.Read(buffer1, 0, buffer1.Length);
                        }

                        countZip++;

                        if (countZip % 50 == 0)
                        {
                            LoaderProgress(3);
                        }
                    }
                }
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                statPageLoaded = true;
                myWB.Base = "wp8";
                myWB.Navigate(new Uri("engine_wp8_nativ.html", UriKind.Relative));
            });
        }
    }
}