using Interpic.Alerts;
using Interpic.Models.Behaviours;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Behaviours.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows.Behaviours
{
    /// <summary>
    /// Interaction logic for AddBehaviour.xaml
    /// </summary>
    public partial class AddBehaviour : Window
    {
        private readonly List<Action> availableActions;
        private readonly List<Behaviour> availableWebBehaviours;
        private readonly bool edit;
        private ObservableCollection<Action> webActions = new ObservableCollection<Action>();
        public Behaviour WebBehaviour { get; set; }

        public AddBehaviour(List<Action> availableActions, List<Behaviour> availableBehaviours, bool edit = false)
        {
            InitializeComponent();
            this.availableActions = availableActions;
            this.availableWebBehaviours = availableBehaviours;
            this.edit = edit;

            lsbWebActions.ItemsSource = webActions;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateWebBehaviour())
            {
                if (!edit)
                {
                    WebBehaviour = new Behaviour();
                }

                WebBehaviour.Name = tbName.Text;
                WebBehaviour.Description = tbDescription.Text;
                WebBehaviour.Actions = webActions.ToList();
                Close();
            }
        }

        private bool ValidateWebBehaviour()
        {
            List<string> errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                errors.Add("Name is required.");
            }

            if (tbDescription.Text == null)
            {
                tbDescription.Text = string.Empty;
            }

            if (webActions.Any() == false)
            {
                errors.Add("At least 1 web action needs to be present");
            }

            if (errors.Any())
            {
                WarningAlert.Show(string.Join("\n", errors));
                return false;
            }

            return true;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            Action webAction = (Action)lsbWebActions.SelectedItem;
            if (ConfirmAlert.Show($"Web action '{webAction.Name}' will be removed.").DialogResult.Value)
            {
                webActions.Remove(webAction);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AddAction addWebActionDialog = new AddAction(availableActions, availableWebBehaviours);
            addWebActionDialog.ShowDialog();
            if (addWebActionDialog.WebAction != null)
            {
                webActions.Add(addWebActionDialog.WebAction);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddAction editWebActionDialog = new AddAction(availableActions, availableWebBehaviours, true);
            editWebActionDialog.WebAction = (Action)lsbWebActions.SelectedItem;
            editWebActionDialog.ShowDialog();
            if (editWebActionDialog.WebAction != null)
            {
                webActions[webActions.IndexOf((Action)lsbWebActions.SelectedItem)] = editWebActionDialog.WebAction;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            WebBehaviour = null;
            Close();
        }

        private void LsbWebActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemove.IsEnabled = lsbWebActions.SelectedItem != null;
            btnEdit.IsEnabled = lsbWebActions.SelectedItem != null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (edit)
            {
                tbName.Text = WebBehaviour.Name;
                tbDescription.Text = WebBehaviour.Description;
                webActions = new ObservableCollection<Action>(WebBehaviour.Actions);
            }
        }
    }
}
