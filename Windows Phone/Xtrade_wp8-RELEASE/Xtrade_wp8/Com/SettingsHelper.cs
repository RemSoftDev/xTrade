using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Wallet;

namespace Xtrade_wp8.Com
{
    public static class SettingsHelper
    {
        private static IsolatedStorageSettings settings;

        private const string LoginKey = "Login";

        private const string PasswordKey = "Password";

        private const string DataJsonKey = "dataJson";

        static SettingsHelper()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(LoginKey))
            {
                settings.Add(LoginKey, string.Empty);
                settings.Save();
            }

            if (!settings.Contains(PasswordKey))
            {
                settings.Add(PasswordKey, string.Empty);
                settings.Save();
            }

            if (!settings.Contains(DataJsonKey))
            {
                settings.Add(DataJsonKey, string.Empty);
                settings.Save();
            }
        }

        public static string Login
        {
            get
            {
                return settings[LoginKey] as string;
            }

            set
            {
                settings[LoginKey] = value;
                settings.Save();
            }
        }

        public static string Password
        {
            get
            {
                return settings[PasswordKey] as string;
            }

            set
            {
                settings[PasswordKey] = value;
                settings.Save();
            }
        }

        public static string DataJson
        {
            get
            {
                return settings[DataJsonKey] as string;
            }

            set
            {
                settings[DataJsonKey] = value;
                settings.Save();
            }
        }
    }
}
