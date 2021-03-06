﻿using System.Windows;
using System.Windows.Controls;

namespace Interpic.Settings.Controls
{
    /// <summary>
    /// Interaction logic for TextSetting.xaml
    /// </summary>
    public partial class TextSetting : UserControl
    {
        private Setting<string> setting;
        public TextSetting(Setting<string> setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            tbValue.Text = setting.Value;
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
            HelpResult<string> result = setting.Helper.Help(setting.Value);
            if (!result.Canceled)
            {
                tbValue.Text = result.Result;
            }
        }

        public bool Validate()
        {
            if (setting.Validator != null)
            {
                SettingValidationResult result = setting.Validator.Validate(tbValue.Text);
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

        public string GetValue()
        {
            return tbValue.Text;
        }
    }
}
