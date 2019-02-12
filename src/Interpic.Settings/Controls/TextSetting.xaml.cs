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
