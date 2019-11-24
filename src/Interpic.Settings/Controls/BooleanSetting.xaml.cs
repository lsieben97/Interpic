using System.Windows;
using System.Windows.Controls;

namespace Interpic.Settings.Controls
{
    /// <summary>
    /// Interaction logic for BooleanSetting.xaml
    /// </summary>
    public partial class BooleanSetting : UserControl
    {
        private Setting<bool> setting;
        public BooleanSetting(Setting<bool> setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            cbValue.IsChecked = setting.Value;
            if (setting.Helper != null)
            {
                cbValue.Visibility = Visibility.Collapsed;
                btnHelp.Visibility = Visibility.Visible;
                btnHelp.Content = setting.Helper.HelpButtonText;
                btnHelp.Click += BtnHelp_Click;
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpResult<bool> result = setting.Helper.Help(setting.Value);
            if (!result.Canceled)
            {
                cbValue.IsChecked = result.Result;
            }
        }

        public bool Validate()
        {
            if (setting.Validator != null)
            {
                SettingValidationResult result = setting.Validator.Validate(cbValue.IsChecked.Value);
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

        public bool GetValue()
        {
            return cbValue.IsChecked.Value;
        }
    }
}
