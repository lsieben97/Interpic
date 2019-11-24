using Interpic.Alerts;
using Interpic.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Studio.Controls
{
    /// <summary>
    /// Interaction logic for PageListItem.xaml
    /// </summary>
    public partial class PageListItem : UserControl
    {
        private readonly IStudioEnvironment studio;

        public PageListItem(Models.Page page, IStudioEnvironment studio)
        {
            InitializeComponent();
            Page = page;
            this.studio = studio;
            lbPageName.Text = page.Name;
            Page.PropertyChanged += PagePropertyChanged;
            btnGetManualSource.Visibility = Page.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void PagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoaded")
            {
                btnGetManualSource.Visibility = Page.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }

        public Models.Page Page { get; }

        private void BtnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Page);
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowManualItemSettings(Page);
        }

        private void BtnGetManualSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.LoadManualItem(Page);
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.RemoveManualItem(Page, true);
        }

        private void LbPageName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Page);
        }
    }
}
