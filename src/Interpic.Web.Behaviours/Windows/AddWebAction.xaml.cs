using Interpic.Alerts;
using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
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

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for AddWebAction.xaml
    /// </summary>
    public partial class AddWebAction : Window
    {
        private readonly List<WebAction> availableActions;
        private readonly List<WebBehaviour> availableBehaviours;
        private readonly bool edit;
        private SettingsCollection parameters;

        public WebAction WebAction { get; set; }
        public AddWebAction(List<WebAction> availableActions, List<WebBehaviour> availableBehaviours, bool edit = false)
        {
            InitializeComponent();
            this.availableActions = availableActions;
            this.availableBehaviours = availableBehaviours;
            this.edit = edit;
            cbbType.ItemsSource = availableActions;
            availableBehaviours.Insert(0, WebBehaviour.None);
            cbbBehaviourWhenFalse.ItemsSource = availableBehaviours;
            cbbBehaviourWhenTrue.ItemsSource = availableBehaviours;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (edit)
            {
                cbbType.SelectedValue = WebAction.Id;
                parameters = WebAction.Parameters;
                btnSetParameters.IsEnabled = parameters == null;
                if (WebAction.Type == WebAction.WebActionType.Check)
                {
                    CheckWebAction checkWebAction = WebAction as CheckWebAction;
                    cbbBehaviourWhenFalse.SelectedItem = checkWebAction.BehaviourWhenFalse;
                    cbbBehaviourWhenTrue.SelectedItem = checkWebAction.BehaviourWhenTrue;
                }
                else
                {
                    cbbBehaviourWhenFalse.IsEnabled = false;
                    cbbBehaviourWhenTrue.IsEnabled = false;
                }
            }
            else
            {
                cbbBehaviourWhenFalse.SelectedIndex = 0;
                cbbBehaviourWhenTrue.SelectedIndex = 0;
                cbbType.SelectedIndex = 0;
            }
        }

        private void CbbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebAction webAction = availableActions.Find(webaction => webaction.Id == cbbType.SelectedValue.ToString());
            webAction.Parameters = webAction.GetDefaultParameters();
            parameters = webAction.Parameters;
            
            btnSetParameters.IsEnabled = parameters != null;
            if (webAction.Type == WebAction.WebActionType.Action)
            {
                cbbBehaviourWhenFalse.IsEnabled = false;
                cbbBehaviourWhenTrue.IsEnabled = false;
            }
            else
            {
                cbbBehaviourWhenFalse.IsEnabled = true;
                cbbBehaviourWhenTrue.IsEnabled = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            WebAction = null;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (parameters.Validate())
            {
                WebAction = availableActions.Find(webaction => webaction.Id == cbbType.SelectedValue.ToString());
                WebAction.Parameters = parameters;

                if (WebAction.Type == WebAction.WebActionType.Check)
                {
                    CheckWebAction checkWebAction = WebAction as CheckWebAction;
                    checkWebAction.BehaviourWhenFalse = (WebBehaviour)cbbBehaviourWhenFalse.SelectedItem;
                    checkWebAction.BehaviourWhenTrue = (WebBehaviour)cbbBehaviourWhenTrue.SelectedItem;
                    WebAction = checkWebAction;
                }
                Close();
            }
            else
            {
                WarningAlert.Show("Not all parameters have been set!");
            }
        }

        private void BtnSetParameters_Click(object sender, RoutedEventArgs e)
        {
            SettingsEditor settingsEditor = new SettingsEditor(parameters, new BitmapImage(new Uri("/Interpic.Web.Icons;component/Icons/BehaviourWhite.png", UriKind.RelativeOrAbsolute)), "Edit Parameters", "Parameter description");
            bool? result = settingsEditor.ShowDialog();
            if (result.HasValue)
            {
                if (result.Value)
                {
                    parameters = settingsEditor.SettingsCollection;
                }
            }
            
        }
    }
}
