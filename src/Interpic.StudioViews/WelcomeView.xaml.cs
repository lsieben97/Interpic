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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interpic.StudioViews
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl, IStudioViewHandler
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public IStudioEnvironment Studio { get; set; }

        public void ViewAttached(bool firstTime)
        {
        }
    }
}
