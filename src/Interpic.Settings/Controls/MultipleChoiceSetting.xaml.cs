using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Settings.Controls
{
    /// <summary>
    /// Interaction logic for MultipleChoiceSetting.xaml
    /// </summary>
    public partial class MultipleChoiceSetting : UserControl
    {
        private Settings.MultipleChoiceSetting setting;
        public MultipleChoiceSetting(Settings.MultipleChoiceSetting setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            foreach (KeyValuePair<string, string> choice in setting.Choices)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = choice.Key;
                item.Tag = choice.Value;
                cbbValue.Items.Add(item);
            }
            cbbValue.SelectedItem = GetComboBoxItemForValue(setting.Value);
            if (setting.Helper != null)
            {
                cbbValue.Visibility = Visibility.Collapsed;
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
                cbbValue.SelectedItem = GetComboBoxItemForValue(result.Result);
            }
        }

        public bool Validate()
        {
            if (setting.Validator != null)
            {
                SettingValidationResult result = setting.Validator.Validate(GetValue());
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
            return ((ComboBoxItem)cbbValue.SelectedItem).Tag.ToString();
        }

        private ComboBoxItem GetComboBoxItemForValue(string value)
        {
            foreach (ComboBoxItem item in cbbValue.Items)
            {
                if (item.Tag.ToString() == value)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
