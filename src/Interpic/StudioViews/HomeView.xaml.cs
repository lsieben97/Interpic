using Interpic.Models;
using Interpic.UI.Controls;
using System.Windows.Controls;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl, IStudioViewHandler
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }


        public void ViewAttached()
        {
            lbVersion.Text = "V" + Studio.GetStudioVersion();
        }

        public void ViewDetached()
        {
            
        }

        public string GetTabContents()
        {
            return "Home";
        }
    }
}
