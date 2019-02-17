using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for NewVersion.xaml
    /// </summary>
    public partial class NewVersion : Window
    {
        private SettingsCollection versionSettings;
        private bool edit = false;
        public Models.Version Version { get; set; }
        public NewVersion(SettingsCollection versionSettings)
        {
            this.versionSettings = versionSettings;
            InitializeComponent();

            if (versionSettings == null)
            {
                btnShowOptions.IsEnabled = false;
            }
            LoadLocales();
        }

        public NewVersion(SettingsCollection versionSettings, Models.Version version)
        {
            this.versionSettings = versionSettings;
            InitializeComponent();

            if (versionSettings == null)
            {
                btnShowOptions.IsEnabled = false;
            }

            LoadLocales();

            foreach(ComboBoxItem item in cbLocales.Items)
            {
                if (item.Tag.ToString() == version.Locale)
                {
                    item.IsSelected = true;
                }
            }
            tbName.Text = version.Name;
            lbTitle.Text = "Edit version";
            btnCreate.Content = "Save";
        }

        private void LoadLocales()
        {
            foreach(CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = info.EnglishName;
                item.Tag = info.Name;
                cbLocales.Items.Add(item);
            }
            cbLocales.SelectedIndex = 0;
        }
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (! edit)
            {
                Version = new Models.Version();
            }
            if (tbName.Text.Length > 0)
            {
                Version.Name = tbName.Text;
                Version.Locale = (cbLocales.SelectedItem as ComboBoxItem).Tag.ToString();
                Version.Settings = versionSettings;
                Version.Pages = new System.Collections.ObjectModel.ObservableCollection<Models.Page>();
                DialogResult = true;
                Close();
            }
            else
            {
                lbNameError.Text = "Enter a name";
            }
        }

        private void BtnShowOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsEditor editor = new SettingsEditor(versionSettings);
            editor.ShowDialog();
            if (editor.DialogResult.Value)
            {
                versionSettings = editor.SettingsCollection;
            }
        }
    }
}
