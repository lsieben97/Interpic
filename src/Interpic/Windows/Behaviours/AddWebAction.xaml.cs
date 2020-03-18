using Interpic.Alerts;
using Interpic.Models.Behaviours;
using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for AddWebAction.xaml
    /// </summary>
    public partial class AddAction : Window
    {
        private readonly List<Interpic.Models.Behaviours.Action> availableActions;
        private readonly List<Behaviour> availableBehaviours;
        private readonly bool edit;
        private SettingsCollection parameters;

        public Interpic.Models.Behaviours.Action WebAction { get; set; }
        public AddAction(List<Interpic.Models.Behaviours.Action> availableActions, List<Behaviour> availableBehaviours, bool edit = false)
        {
            InitializeComponent();
            this.availableActions = availableActions;
            this.availableBehaviours = availableBehaviours;
            this.edit = edit;
            cbbType.ItemsSource = availableActions;
            availableBehaviours.Insert(0, Behaviour.None);
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
                if (WebAction.Type == Interpic.Models.Behaviours.Action.ActionType.Check)
                {
                    CheckAction checkWebAction = WebAction as CheckAction;
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
            Interpic.Models.Behaviours.Action webAction = availableActions.Find(webaction => webaction.Id == cbbType.SelectedValue.ToString());
            webAction.Parameters = webAction.GetDefaultParameters();
            parameters = webAction.Parameters;
            
            btnSetParameters.IsEnabled = parameters != null;
            if (webAction.Type == Interpic.Models.Behaviours.Action.ActionType.Action)
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

                if (WebAction.Type == Interpic.Models.Behaviours.Action.ActionType.Check)
                {
                    CheckAction checkWebAction = WebAction as CheckAction;
                    checkWebAction.BehaviourWhenFalse = (Behaviour)cbbBehaviourWhenFalse.SelectedItem;
                    checkWebAction.BehaviourWhenTrue = (Behaviour)cbbBehaviourWhenTrue.SelectedItem;
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
            SettingsEditor settingsEditor = new SettingsEditor(parameters, new BitmapImage(new Uri("/Interpic.UI;component/Icons/BehaviourWhite.png", UriKind.RelativeOrAbsolute)), "Edit Parameters", "Parameter description");
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
