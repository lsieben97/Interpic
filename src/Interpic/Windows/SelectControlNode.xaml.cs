using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
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
using System.Xml;

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
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
                ErrorAlert.Show("Please select a node.");
                return;
            }
            Close();
        }
    }
}
