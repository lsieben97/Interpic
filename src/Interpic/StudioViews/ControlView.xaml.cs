using Interpic.Models;
using Interpic.Studio.Windows.Behaviours;
using Interpic.UI.Controls;
using Interpic.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for ControlView.xaml
    /// </summary>
    public partial class ControlView : UserControl, IStudioViewHandler
    {
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        public Models.Control Control { get; }

        public ControlView(Models.Control control)
        {
            InitializeComponent();
            Control = control;
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.ShowManualItemSettings(Control);
        }

        private void BtnGetManualSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.LoadManualItem(Control);
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.RemoveManualItem(Control, true);
        }

        private void BtnSaveControlDescription_Click(object sender, RoutedEventArgs e)
        {
            StudioTab.ContainsChanges(false);
            Control.Description = tbControlDescription.Text;
        }

        private void TbControlDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            StudioTab.ContainsChanges(true);
        }

        public void ViewAttached() { }

        public void ViewDetached()
        {
            Control.PropertyChanged -= ControlPropertyChanged;
        }

        public string GetTabContents()
        {
            return Control.Id;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StudioTab.SetTitle(Control.Name);
            tbControlDescription.Text = Control.Description;
            Control.PropertyChanged += ControlPropertyChanged;
            btnRefresh.Visibility = Control.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description")
            {
                if(StudioTab.DoesContainChanges == false)
                {
                    tbControlDescription.Text = Control.Description;
                }
            }
            else if(e.PropertyName == "IsLoaded")
            {
                btnRefresh.Visibility = Control.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void btnSelectBehaviours_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PickBehaviours pickBehavioursWindow = new PickBehaviours(Studio.GetBehaviourConfiguration().Behaviours, Control.Behaviours);
            pickBehavioursWindow.ShowDialog();
            Control.Behaviours = pickBehavioursWindow.SelectedBehaviours;
            Control.BehaviourIds = BehaviourUtils.GetBehaviourIds(Control.Behaviours);
        }
    }
}
