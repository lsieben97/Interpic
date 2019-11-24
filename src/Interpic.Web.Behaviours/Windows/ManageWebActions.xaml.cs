using Interpic.Alerts;
using Interpic.Web.Behaviours.Models;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for ManageWebActions.xaml
    /// </summary>
    public partial class ManageWebActions : Window
    {
        public WebBehaviourConfiguration Configuration { get; set; }

        public ManageWebActions()
        {
            InitializeComponent();
        }

        public ManageWebActions(WebBehaviourConfiguration configuration)
        {
            InitializeComponent();
            Configuration = configuration;
            lsbPacks.ItemsSource = Configuration.InternalWebActionPacks;
        }

        private void LsbPacks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsbPacks.SelectedItem != null)
            {
                lsbActions.ItemsSource = ((WebActionPack)lsbPacks.SelectedItem).GetActions();
            }
        }

        private void LsbActions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lsbActions.SelectedItem != null)
            {
                WebAction action = (WebAction)lsbActions.SelectedItem;
                InfoAlert.Show($"{action.Name}\n\n{action.Description}\n\nType:\n{action.Type.ToString()}\n\nParameters:\n{GetParametersForAction(action)}");
            }
        }

        private string GetParametersForAction(WebAction action)
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
