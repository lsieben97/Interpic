using Interpic.Alerts;
using Interpic.Web.Behaviours.Models;
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
using System.Windows.Shapes;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for AddBehaviour.xaml
    /// </summary>
    public partial class AddBehaviour : Window
    {
        private readonly List<WebAction> availableActions;
        private readonly List<WebBehaviour> availableWebBehaviours;
        private readonly bool edit;
        private ObservableCollection<WebAction> webActions = new ObservableCollection<WebAction>();
        public WebBehaviour WebBehaviour { get; set; }

        public AddBehaviour(List<WebAction> availableActions, List<WebBehaviour> availableWebBehaviours, bool edit = false)
        {
            InitializeComponent();
            this.availableActions = availableActions;
            this.availableWebBehaviours = availableWebBehaviours;
            this.edit = edit;
            
            if (edit)
            {
                tbName.Text = WebBehaviour.Name;
                tbDescription.Text = WebBehaviour.Description;
                webActions = new ObservableCollection<WebAction>(WebBehaviour.Actions);
            }

            lsbWebActions.ItemsSource = webActions;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateWebBehaviour())
            {
                if (!edit)
                {
                    WebBehaviour = new WebBehaviour();
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
            WebAction webAction = (WebAction)lsbWebActions.SelectedItem;
            if (ConfirmAlert.Show($"Web action '{webAction.Name}' will be removed.").DialogResult.Value)
            {
                webActions.Remove(webAction);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AddWebAction addWebActionDialog = new AddWebAction(availableActions, availableWebBehaviours);
            addWebActionDialog.ShowDialog();
            if (addWebActionDialog.WebAction != null)
            {
                webActions.Add(addWebActionDialog.WebAction);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddWebAction editWebActionDialog = new AddWebAction(availableActions, availableWebBehaviours, true);
            editWebActionDialog.WebAction = (WebAction)lsbWebActions.SelectedItem;
            editWebActionDialog.ShowDialog();
            if (editWebActionDialog.WebAction != null)
            {
                webActions[webActions.IndexOf((WebAction)lsbWebActions.SelectedItem)] = editWebActionDialog.WebAction;
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
    }
}
