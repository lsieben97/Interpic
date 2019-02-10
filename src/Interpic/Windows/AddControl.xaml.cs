using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Utils;
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
    /// Interaction logic for AddControl.xaml
    /// </summary>
    public partial class AddControl : Window
    {
        public Models.Control Control { get; set; }
        private SelectControlNode NodeSelector;
        private bool edit = false;

        public AddControl(ObservableCollection<DiscoveredControl> discoveredControls, IControlIdentifierSelector manualSelector)
        {
            InitializeComponent();
            NodeSelector = new SelectControlNode(discoveredControls, manualSelector);
        }

        public AddControl(Models.Control control, IControlIdentifierSelector manualSelector)
        {
            InitializeComponent();
            Control = control;
            lbTitle.Text = "Edit Control";
            btnCreate.Content = "Save";
            edit = true;
            tbName.Text = Control.Name;
            NodeSelector = new SelectControlNode(control.Parent.DiscoveredControls, manualSelector);
            NodeSelector.SelectedNode = control.Identifier;
            tbBaseNode.Text = control.Identifier.Name;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (tbName.Text == string.Empty)
            {
                valid = false;
                lbNameError.Text = "Enter name";
            }
            else
            {
                lbNameError.Text = string.Empty;
            }

            if (valid == true)
            {
                if (!edit)
                {
                    Control = new Models.Control();  
                }
                Control.Name = tbName.Text;
                Control.Identifier = NodeSelector.SelectedNode;
                Close();
            }
        }

        private void btnSelectNode_Click(object sender, RoutedEventArgs e)
        {
            NodeSelector.ShowDialog();
            if (NodeSelector.SelectedNode != null)
            {
                    tbBaseNode.Text = NodeSelector.SelectedNode.Name;
            }
        }
    }
}
