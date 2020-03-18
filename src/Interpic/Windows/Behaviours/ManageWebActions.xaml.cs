using Interpic.Alerts;
using Interpic.Models.Behaviours;
using Interpic.Web.Behaviours.Models;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Studio.Windows.Behaviours
{
    /// <summary>
    /// Interaction logic for ManageWebActions.xaml
    /// </summary>
    public partial class ManageActions : Window
    {
        public BehaviourConfiguration Configuration { get; set; }

        public ManageActions()
        {
            InitializeComponent();
        }

        public ManageActions(BehaviourConfiguration configuration)
        {
            InitializeComponent();
            Configuration = configuration;
            lsbPacks.ItemsSource = Configuration.InternalWebActionPacks;
        }

        private void LsbPacks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsbPacks.SelectedItem != null)
            {
                lsbActions.ItemsSource = ((ActionPack)lsbPacks.SelectedItem).GetActions();
            }
        }

        private void LsbActions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lsbActions.SelectedItem != null)
            {
                Action action = (Action)lsbActions.SelectedItem;
                InfoAlert.Show($"{action.Name}\n\n{action.Description}\n\nType:\n{action.Type.ToString()}\n\nParameters:\n{GetParametersForAction(action)}");
            }
        }

        private string GetParametersForAction(Action action)
        {
            if (action.Parameters == null)
            {
                return "None";
            }

            StringBuilder parameters = new StringBuilder();

            parameters.Append(string.Join("\n", action.Parameters.TextSettings.Select(setting => setting.Name)));
            parameters.Append(string.Join("\n", action.Parameters.NumeralSettings.Select(setting => setting.Name)));
            parameters.Append(string.Join("\n", action.Parameters.PathSettings.Select(setting => setting.Name)));
            parameters.Append(string.Join("\n", action.Parameters.BooleanSettings.Select(setting => setting.Name)));
            parameters.Append(string.Join("\n", action.Parameters.MultipleChoiceSettings.Select(setting => setting.Name)));
            parameters.Append(string.Join("\n", action.Parameters.SubSettings.Select(setting => setting.Name)));
            return parameters.ToString();
        }
    }
}
