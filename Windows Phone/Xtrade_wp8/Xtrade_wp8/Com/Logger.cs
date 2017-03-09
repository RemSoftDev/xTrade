using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Xtrade_wp8.Com
{
    class Logger
    {
       

        public static void Show(string msg, bool isConsole = true)
        {
            if (Constants.IS_DEBUG == true)
            {
                if (isConsole)
                {
                    Debug.WriteLine(msg);
                }
                else {
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }

        }
    }
}
