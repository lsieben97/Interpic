using Interpic.Alerts;
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

namespace Interpic.Studio.Windows
{
    /// <summary>
    /// Interaction logic for NewPage.xaml
    /// </summary>
    public partial class NewPage : Window
    {
        public Models.Page Page { get; set; }
        private bool edit = false;

        public NewPage()
        {
            InitializeComponent();
            tbTypeDescription.Text = "Reference page to describe sections and controls.\nA normal Interpic page.";
            cbbType.SelectedIndex = 0;
        }

        public NewPage(Models.Page page)
        {
            InitializeComponent();
            Page = page;
            lbTitle.Text = "Edit project";
            btnCreate.Content = "Save";
            edit = true;
            tbName.Text = Page.Name;
            if (Page.Type == "text")
            {
                cbbType.SelectedIndex = 1;
            }
            else if (Page.Type == "reference")
            {
                cbbType.SelectedIndex = 0;
            }
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
                    Page = new Models.Page();
                    Page.Name = tbName.Text;
                    Page.Type = ((ComboBoxItem)cbbType.SelectedItem).Tag.ToString();
                    Page.Sections = new ObservableCollection<Models.Section>();
                    Close();
                }
                else
                {
                    Page.Name = tbName.Text;
                    string type = ((ComboBoxItem)cbbType.SelectedItem).Tag.ToString();
                    if (type != Page.Type)
                    {
                        ConfirmAlert alert = ConfirmAlert.Show("Changing the page type will remove any content on the page.");
                        if (alert.Result == true)
                        {
                            Page.Type = type;
                            Page.Sections = new ObservableCollection<Models.Section>();
                            Page.Description = string.Empty;
                        }
                    }
                    Close();
                }
            }
        }

        private void cbbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbType.SelectedIndex == 0)
            {
                tbTypeDescription.Text = "Reference page to describe sections and controls.\nA normal Interpic page.";
            }
            else if (cbbType.SelectedIndex == 1)
            {
                tbTypeDescription.Text = "Text page to enter text in the output format of the project.\nThen page will be rendered like every other page.";
            }
        }
    }
}
