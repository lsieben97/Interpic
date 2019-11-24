using Interpic.Models.Extensions;
using Interpic.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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

            if (tbBaseNode.Text == string.Empty)
            {
                valid = false;
                lbNodeError.Text = "Enter node";
            }
            else
            {
                lbNodeError.Text = string.Empty;
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Control = null;
            Close();
        }
    }
}
