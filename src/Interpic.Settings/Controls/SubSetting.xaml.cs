using System.Windows;
using System.Windows.Controls;

namespace Interpic.Settings.Controls
{
    /// <summary>
    /// Interaction logic for SubSetting.xaml
    /// </summary>
    public partial class SubSetting : UserControl
    {
        private Setting<SettingsCollection> setting;
        public SubSetting(Setting<SettingsCollection> setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            btnOpenSubSettings.Content = "Open " + setting.Name;
            btnOpenSubSettings.Click += BtnOpenSubSettings_Click;
            if (setting.Helper != null)
            {
                btnOpenSubSettings.Visibility = Visibility.Collapsed;
                btnHelp.Visibility = Visibility.Visible;
                btnHelp.Content = setting.Helper.HelpButtonText;
                btnHelp.Click += BtnHelp_Click;
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpResult<SettingsCollection> result = setting.Helper.Help(setting.Value);
            if (!result.Canceled)
            {
                setting.Value = result.Result;
            }
        }

        private void BtnOpenSubSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsEditor editor = new SettingsEditor(setting.Value);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                setting.Value = editor.SettingsCollection;
            }
        }

        public bool Validate()
        {
            if (setting.Validator != null)
            {
                SettingValidationResult result = setting.Validator.Validate(setting.Value);
                if (result.IsValid == false)
                {
                    lbError.Text = result.ErrorMessage;
                }
                return result.IsValid;
            }
            else
            {
                return true;
            }
        }

        public SettingsCollection GetValue()
        {
            return setting.Value;
        }
    }
}
