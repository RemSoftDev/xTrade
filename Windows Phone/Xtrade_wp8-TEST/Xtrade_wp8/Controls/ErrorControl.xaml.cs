using System.Windows;
using System.Windows.Input;
//using GalaSoft.MvvmLight.Messaging;
using Xtrade_wp8.Com.Messages;

namespace Xtrade_wp8.Controls
{
    public partial class ErrorControl
    {
        public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.RegisterAttached("ErrorText", typeof(string), typeof(ErrorControl), new PropertyMetadata(null));

        public string ErrorText
        {
            get
            {
                return (string)this.GetValue(ErrorTextProperty);
            }

            set
            {
                this.SetValue(ErrorTextProperty, value);
            }
        }

        public ErrorControl()
        {
            InitializeComponent();
        }

        private void OkButton_OnTap(object sender, GestureEventArgs e)
        {
           // Messenger.Default.Send(new CloseErrorControlMessage());
        }
    }
}