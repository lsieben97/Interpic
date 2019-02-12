using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
