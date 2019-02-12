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
