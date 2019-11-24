using Interpic.Alerts;
using Interpic.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Studio.Controls
{
    /// <summary>
    /// Interaction logic for PageListItem.xaml
    /// </summary>
    public partial class ControlListItem : UserControl
    {
        private readonly IStudioEnvironment studio;

        public ControlListItem(Models.Control control, IStudioEnvironment studio)
        {
            InitializeComponent();
            Control = control;
            this.studio = studio;
            lbControlName.Text = control.Name;
            Control.PropertyChanged += ControlPropertyChanged;
            btnRefresh.Visibility = Control.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void ControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            btnRefresh.Visibility = Control.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public Models.Control Control { get; }

        private void BtnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Control);
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowManualItemSettings(Control);
        }

        private void BtnRefresh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.LoadManualItem(Control);
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.RemoveManualItem(Control, true);
        }

        private void LbPageName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Control);
        }
    }
}
