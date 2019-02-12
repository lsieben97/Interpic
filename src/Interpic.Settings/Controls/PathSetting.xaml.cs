using Microsoft.Win32;
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
    /// Interaction logic for PathSetting.xaml
    /// </summary>
    public partial class PathSetting : UserControl
    {
        private Settings.PathSetting setting;
        public PathSetting(Settings.PathSetting setting)
        {
            InitializeComponent();
            this.setting = setting;
            lbTitle.Text = setting.Name;
            this.Tag = setting.Description;
            tbValue.Text = setting.Value.ToString();
            btnSelect.Click += PathButton_Click;
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
        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            if (setting.Type == Settings.PathSetting.PathType.Folder)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (setting.StartPath != null)
                {
                    dialog.SelectedPath = setting.StartPath;
                }
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tbValue.Text = dialog.SelectedPath;
                }
            }
            else if (setting.Type == Settings.PathSetting.PathType.File)
            {
                if (setting.Operation == Settings.PathSetting.PathOperation.Save)
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Title = setting.DialogTitle;
                    dialog.FileName = setting.StartFileName;
                    dialog.DefaultExt = setting.Extension;
                    dialog.Filter = setting.Filter;
                    bool? result = dialog.ShowDialog();
                    if (result.HasValue)
                    {
                        if (result.Value == true)
                        {
                            tbValue.Text = dialog.FileName;
                        }
                    }
                }
                else if (setting.Operation == Settings.PathSetting.PathOperation.Load)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = setting.DialogTitle;
                    dialog.FileName = setting.StartFileName;
                    dialog.DefaultExt = setting.Extension;
                    dialog.Filter = setting.Filter;
                    bool? result = dialog.ShowDialog();
                    if (result.HasValue)
                    {
                        if (result.Value == true)
                        {
                            tbValue.Text = dialog.FileName;
                        }
                    }
                }
            }
        }
        public string GetValue()
        {
            return tbValue.Text;
        }
    }
}
