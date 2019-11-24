using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows.Selectors
{
    /// <summary>
    /// Interaction logic for SelectPage.xaml
    /// </summary>
    public partial class SelectPage : Window
    {
        public string SelectedPageId { get; set; }
        public SelectPage(Models.Version version)
        {
            InitializeComponent();
            LoadPages(version);
        }

        private void LoadPages(Models.Version version)
        {
            foreach(Models.Page page in version.Pages)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = page.Name;
                item.Tag = page.Id;
                lsbPages.Items.Add(item);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageId = null;
            DialogResult = false;
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageId = ((ListBoxItem)lsbPages.SelectedItem).Tag.ToString();
            DialogResult = true;
        }
    }
}
