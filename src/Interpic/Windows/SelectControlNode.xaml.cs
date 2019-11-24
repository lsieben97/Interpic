using Interpic.Alerts;
using Interpic.Models.Extensions;
using Interpic.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows
{
    /// <summary>
    /// Interaction logic for SelectControlNode.xaml
    /// </summary>
    public partial class SelectControlNode : Window
    {
        public ControlIdentifier SelectedNode;
        private ObservableCollection<DiscoveredControl> discoveredControls;
        private IControlIdentifierSelector manualSelector;
        public SelectControlNode(ObservableCollection<DiscoveredControl> discoveredControls, IControlIdentifierSelector manualSelector)
        {
            InitializeComponent();
            this.discoveredControls = discoveredControls;
            this.manualSelector = manualSelector;
            LoadDiscoveredControls();
        }

        private void LoadDiscoveredControls()
        {
            foreach(DiscoveredControl control in discoveredControls)
            {
                ListBoxItem item = new ListBoxItem();
                string name = control.Name;
                item.Content = name;
                item.Tag = control.Identifier;
                lsbDiscoveredControls.Items.Add(item);
            }
        }

        private void lsbDiscoveredControls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedNode = (lsbDiscoveredControls.SelectedItem as ListBoxItem).Tag as ControlIdentifier;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedNode = null;
            Close();
        }

        private void btnSelectCustomNode_Click(object sender, RoutedEventArgs e)
        {
            manualSelector.ShowSelector();
            SelectedNode = manualSelector.ControlIdentifier;
            FinishSelection();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            FinishSelection();
        }

        private void FinishSelection()
        {
            if (SelectedNode == null)
            {
                WarningAlert.Show("Please select a node.");
                return;
            }
            Close();
        }
    }
}
