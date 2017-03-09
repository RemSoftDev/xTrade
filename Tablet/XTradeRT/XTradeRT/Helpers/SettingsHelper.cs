using Windows.Storage;

namespace Xtrade_wp8.Com
{
    public static class SettingsHelper
    {
        private static ApplicationDataContainer settings;

        private const string LoginKey = "Login";

        private const string PasswordKey = "Password";

        private const string DataJsonKey = "dataJson";

        private const string ContainerKey = "DataContainer";

        static SettingsHelper()
        {
            settings = ApplicationData.Current.LocalSettings;
            /*if (!settings.Containers.ContainsKey(ContainerKey))
            {
                settings.CreateContainer(LoginKey, ApplicationDataCreateDisposition.Always);
            }*/

            if (!settings.Values.ContainsKey(LoginKey))
            {
                settings.Values[LoginKey] = string.Empty;
            }

            if (!settings.Values.ContainsKey(PasswordKey))
            {
                settings.Values[PasswordKey] = string.Empty;
            }

            if (!settings.Values.ContainsKey(DataJsonKey))
            {
                settings.Values[DataJsonKey] = string.Empty;
            }
        }

        public static string Login
        {
            get
            {
                return settings.Values[LoginKey] as string;
            }

            set
            {
                settings.Values[LoginKey] = value;
            }
        }

        public static string Password
        {
            get
            {
                return settings.Values[PasswordKey] as string;
            }

            set
            {
                settings.Values[PasswordKey] = value;
            }
        }

        public static string DataJson
        {
            get
            {
                return settings.Values[DataJsonKey] as string;
            }

            set
            {
                settings.Values[DataJsonKey] = value;
            }
        }
    }
}
