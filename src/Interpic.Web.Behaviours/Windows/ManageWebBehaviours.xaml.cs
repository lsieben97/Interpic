using Interpic.Alerts;
using Interpic.Models;
using Interpic.UI.Controls;
using Interpic.Web.Behaviours.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for ManageWebBehaviours.xaml
    /// </summary>
    public partial class ManageWebBehaviours : UserControl, IStudioViewHandler
    {
        public WebBehaviourConfiguration Configuration { get; set; }
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        private readonly Project project;
        private List<WebAction> availableWebActions = new List<WebAction>();
        private ObservableCollection<WebBehaviour> behaviours;

        public ManageWebBehaviours(WebBehaviourConfiguration configuration, Project project)
        {
            InitializeComponent();
            this.Configuration = configuration;
            this.project = project;
            foreach (List<WebAction> pack in configuration.InternalWebActionPacks.Select(wp => wp.GetActions()))
            {
                availableWebActions.AddRange(pack);
            }
            behaviours = new ObservableCollection<WebBehaviour>(Configuration.Behaviours);
            lsbBehaviours.ItemsSource = behaviours;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AddBehaviour addBehaviourDialog = new AddBehaviour(availableWebActions, behaviours.ToList());
            addBehaviourDialog.ShowDialog();
            if (addBehaviourDialog.WebBehaviour != null)
            {
                behaviours.Add(addBehaviourDialog.WebBehaviour);
                Configuration.Behaviours.Add(addBehaviourDialog.WebBehaviour);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddBehaviour editBehaviourDialog = new AddBehaviour(availableWebActions, behaviours.ToList(), true);
            editBehaviourDialog.WebBehaviour = (WebBehaviour)lsbBehaviours.SelectedItem;
            editBehaviourDialog.ShowDialog();
            if (editBehaviourDialog.WebBehaviour != null)
            {
                behaviours[behaviours.IndexOf((WebBehaviour)lsbBehaviours.SelectedItem)] = editBehaviourDialog.WebBehaviour;
                if (lsbBehaviours.SelectedItem != null)
                {
                    Configuration.Behaviours[behaviours.IndexOf((WebBehaviour)lsbBehaviours.SelectedItem)] = editBehaviourDialog.WebBehaviour;
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            WebBehaviour webbehaviour = (WebBehaviour)lsbBehaviours.SelectedItem;
            if (ValidateWebBehaviourRemoval(webbehaviour))
            {
                if (ConfirmAlert.Show($"Web behaviour '{webbehaviour.Name}' will be removed.").DialogResult.Value)
                {
                    behaviours.Remove((WebBehaviour)lsbBehaviours.SelectedItem);
                    Configuration.Behaviours.Remove((WebBehaviour)lsbBehaviours.SelectedItem);
                }
            }
            else
            {
                WarningAlert.Show("This web behaviour cannot be removed because it's used in the project.");
            }
        }

        private bool ValidateWebBehaviourRemoval(WebBehaviour behaviour)
        {
            bool valid = true;
            // check settings from pages, sections and controls.
            return valid;
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LsbBehaviours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsbBehaviours.SelectedItems.Count == 1)
            {
                btnExport.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnRemove.IsEnabled = true;
            }
            else if (lsbBehaviours.SelectedItems.Count > 1)
            {
                btnExport.IsEnabled = true;
                btnEdit.IsEnabled = false;
                btnRemove.IsEnabled = false;
            }
            else
            {
                btnExport.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnRemove.IsEnabled = false;
            }
        }

        public void ViewAttached()
        {
            
        }

        public void ViewDetached()
        {
            
        }

        public string GetTabContents()
        {
            return "ManageBehaviours";
        }
    }
}
