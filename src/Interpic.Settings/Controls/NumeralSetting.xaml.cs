using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Settings.Controls
{
    /// <summary>
    /// Interaction logic for NumeralSetting.xaml
    /// </summary>
    public partial class NumeralSetting : UserControl
    {
        private Setting<int> setting;
        public NumeralSetting(Setting<int> setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            tbValue.Text = setting.Value.ToString();
            tbValue.PreviewTextInput += NumberValidationTextBox;
            if (setting.Helper != null)
            {
                tbValue.Visibility = Visibility.Collapsed;
                btnHelp.Visibility = Visibility.Visible;
                btnHelp.Content = setting.Helper.HelpButtonText;
                btnHelp.Click += BtnHelp_Click;
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpResult<int> result = setting.Helper.Help(setting.Value);
            if (!result.Canceled)
            {
                tbValue.Text = result.Result.ToString();
            }
        }

        public bool Validate()
        {
            if (tbValue.Text.Length > 0)
            {
                if (setting.Validator != null)
                {
                    SettingValidationResult result = setting.Validator.Validate(Convert.ToInt32(tbValue.Text));
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
            else
            {
                lbError.Text = "Enter a value";
                return false;
            }
        }

        public int GetValue()
        {
            return Convert.ToInt32(tbValue.Text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
