using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using XPProductKey.Library;

namespace XPProductKey.Editor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private Timer _ReadRegistryTimer;
        public WindowMain()
        {
            InitializeComponent();
            _ReadRegistryTimer = new Timer(1000);
            _ReadRegistryTimer.AutoReset = true;
            _ReadRegistryTimer.Elapsed += UpdateKeyTick;
            _ReadRegistryTimer.Enabled = true;
        }
        private void UpdateKeyTick(object sender, System.EventArgs e)
        {
            String CurrentProductKey = ProductKey.Read();
            Application.Current.Dispatcher.Invoke(new Action(() => { TextBoxCurrentKey.Text = CurrentProductKey; }));
        }

        private void ButtonWriteToRegistry_Click(object sender, RoutedEventArgs e)
        {
            ProductKey.Write(TextBoxNewKey.Text);
        }
    }
}
