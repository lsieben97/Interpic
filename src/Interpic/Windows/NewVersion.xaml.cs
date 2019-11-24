using Interpic.Settings;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Version = null;
            Close();
        }
    }
}
