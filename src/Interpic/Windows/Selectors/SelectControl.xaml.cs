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

namespace Interpic.Studio.Windows.Selectors
{
    /// <summary>
    /// Interaction logic for SelectControl.xaml
    /// </summary>
    public partial class SelectControl : Window
    {
        public string SelectedControlId { get; set; }
        public SelectControl(Models.Section section)
        {
            InitializeComponent();
            LoadControls(section);
        }

        private void LoadControls(Models.Section section)
        {
            foreach (Models.Control control in section.Controls)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = control.Name;
                item.Tag = control.Id;
                lsbControls.Items.Add(item);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedControlId = null;
            DialogResult = false;
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedControlId = ((ListBoxItem)lsbControls.SelectedItem).Tag.ToString();
            DialogResult = true;
        }
    }
}
