using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Interpic.Studio.Windows.Developer
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        private DispatcherTimer refreshTimer = new DispatcherTimer();
        Logger logger;
        public Log(Logger logger)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            this.logger = logger;
            InitializeComponent();
            refreshTimer.Interval = new TimeSpan(0, 0, 5);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
            tbLog.Text = string.Join("", logger.InternalLog);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            tbLog.Text = string.Join("", logger.InternalLog);
        }

        private void TbLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbLog.ScrollToEnd();
        }
    }
}
