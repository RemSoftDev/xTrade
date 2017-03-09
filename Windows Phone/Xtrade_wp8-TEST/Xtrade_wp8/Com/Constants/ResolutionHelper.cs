using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;

namespace Xtrade_wp8.Com
{

    public enum Resolutions { WVGA, WXGA, HD, TABLET };

    class ResolutionHelper
    {


        private static bool IsWvga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool IsHD
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 150;
            }
        }


        // Get current device resolution
        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (IsHD) return Resolutions.HD;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }

        // Define if the device is Windows Phone or Tablet
        public static bool IsWinPhone {
            get {
                bool _IsWinPhone;
                double dpi = DisplayProperties.LogicalDpi;
                double h = Application.Current.Host.Content.ActualHeight;
                double w = Application.Current.Host.Content.ActualWidth;
                double inches;

                if (w > h)
                {
                    inches = h / dpi;
                }
                else
                {
                    inches = w / dpi;
                }


                if (inches > 7){_IsWinPhone = false;}
                else { _IsWinPhone = true; }

                return _IsWinPhone;
            }
        }


        // get device height
        public static double getHeight() 
        {
            double deviceHeight = 0;

            double h = Application.Current.Host.Content.ActualHeight;
            double w = Application.Current.Host.Content.ActualWidth;
            if (w > h)
            {
                deviceHeight = h;
            }
            else
            {
                deviceHeight = w;
            }

            return deviceHeight;
        }


        // get device width
        public static double getWidth()
        {
            double deviceWidth = 0;

            double h = Application.Current.Host.Content.ActualHeight;
            double w = Application.Current.Host.Content.ActualWidth;
            if (w > h)
            {
                deviceWidth = h;
            }
            else
            {
                deviceWidth = w;
            }

            return deviceWidth;
        }


    }
}
