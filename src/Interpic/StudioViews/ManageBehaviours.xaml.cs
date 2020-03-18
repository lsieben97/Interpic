using Interpic.Alerts;
using Interpic.Models;
using Interpic.Models.Behaviours;
using Interpic.Studio.Windows.Behaviours;
using Interpic.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Action = Interpic.Models.Behaviours.Action;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for ManageBehaviours.xaml
    /// </summary>
    public partial class ManageBehavioursView : UserControl, IStudioViewHandler
    {
        public ManageBehavioursView()
        {
            InitializeComponent();
        }

        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }

        private readonly Project project;
        private List<Action> availableWebActions = new List<Action>();
        private ObservableCollection<Behaviour> behaviours;

        public ManageBehavioursView(Project project)
        {
            InitializeComponent();

        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AddBehaviour addBehaviourDialog = new AddBehaviour(availableWebActions, behaviours.ToList());
            addBehaviourDialog.ShowDialog();
            if (addBehaviourDialog.WebBehaviour != null)
            {
                behaviours.Add(addBehaviourDialog.WebBehaviour);
                Studio.GetBehaviourConfiguration().Behaviours.Add(addBehaviourDialog.WebBehaviour);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddBehaviour editBehaviourDialog = new AddBehaviour(availableWebActions, behaviours.ToList(), true);
            editBehaviourDialog.WebBehaviour = (Behaviour)lsbBehaviours.SelectedItem;
            editBehaviourDialog.ShowDialog();
            if (editBehaviourDialog.WebBehaviour != null)
            {
                behaviours[behaviours.IndexOf((Behaviour)lsbBehaviours.SelectedItem)] = editBehaviourDialog.WebBehaviour;
                if (lsbBehaviours.SelectedItem != null)
                {
                    Studio.GetBehaviourConfiguration().Behaviours[behaviours.IndexOf((Behaviour)lsbBehaviours.SelectedItem)] = editBehaviourDialog.WebBehaviour;
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            Behaviour webbehaviour = (Behaviour)lsbBehaviours.SelectedItem;
            if (ValidateWebBehaviourRemoval(webbehaviour))
            {
                if (ConfirmAlert.Show($"Web behaviour '{webbehaviour.Name}' will be removed.").DialogResult.Value)
                {
                    behaviours.Remove((Behaviour)lsbBehaviours.SelectedItem);
                    Studio.GetBehaviourConfiguration().Behaviours.Remove((Behaviour)lsbBehaviours.SelectedItem);
                }
            }
            else
            {
                WarningAlert.Show("This web behaviour cannot be removed because it's used in the project.");
            }
        }

        private bool ValidateWebBehaviourRemoval(Behaviour behaviour)
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
            foreach (List<Action> pack in Studio.GetBehaviourConfiguration().InternalWebActionPacks.Select(wp => wp.GetActions()))
            {
                availableWebActions.AddRange(pack);
            }
            behaviours = new ObservableCollection<Behaviour>(Studio.GetBehaviourConfiguration().Behaviours);
            lsbBehaviours.ItemsSource = behaviours;
        }

        public void ViewDetached()
        {

        }

        public string GetTabContents()
        {
            return "ManageBehaviours";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
