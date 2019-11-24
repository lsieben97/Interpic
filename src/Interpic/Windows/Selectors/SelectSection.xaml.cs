using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows.Selectors
{
    /// <summary>
    /// Interaction logic for SelectSection.xaml
    /// </summary>
    public partial class SelectSection : Window
    {
        public string SelectedSectionId { get; set; }
        public SelectSection(Models.Page page)
        {
            InitializeComponent();
            LoadSections(page);
        }

        private void LoadSections(Models.Page page)
        {
            foreach(Models.Section section in page.Sections)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = section.Name;
                item.Tag = section.Id;
                lsbSections.Items.Add(item);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedSectionId = null;
            DialogResult = false;
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedSectionId = ((ListBoxItem)lsbSections.SelectedItem).Tag.ToString();
            DialogResult = true;
        }
    }
}
