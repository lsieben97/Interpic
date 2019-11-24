using Interpic.Alerts;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Interpic.Settings
{
    /// <summary>
    /// Interaction logic for SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : Window
    {
        public SettingsCollection SettingsCollection { get; set; }

        private bool valid = true;

        // No close button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public SettingsEditor(SettingsCollection settingsCollection)
        {
            SettingsCollection = settingsCollection;
            InitializeComponent();
        }

        public SettingsEditor(SettingsCollection settingsCollection, ImageSource icon)
        {
            SettingsCollection = settingsCollection;
            InitializeComponent();
            Icon = icon;
            imIcon.Source = icon;
        }

        public SettingsEditor(SettingsCollection settingsCollection, ImageSource icon, string caption)
        {
            SettingsCollection = settingsCollection;
            InitializeComponent();
            Icon = icon;
            imIcon.Source = icon;
            lbCaption.Text = caption;
        }

        public SettingsEditor(SettingsCollection settingsCollection, ImageSource icon, string caption, string descriptionCaption)
        {
            SettingsCollection = settingsCollection;
            InitializeComponent();
            Icon = icon;
            imIcon.Source = icon;
            lbCaption.Text = caption;
            lbDescriptionCaption.Text = descriptionCaption;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            lbTitle.Text = "Edit " + SettingsCollection.Name;
            Title = "Edit " + SettingsCollection.Name;
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                foreach (Setting<int> setting in SettingsCollection.NumeralSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderNumeralSetting(setting);
                    }
                }

                foreach (Setting<string> setting in SettingsCollection.TextSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderTextSetting(setting);
                    }
                }

                foreach (Setting<bool> setting in SettingsCollection.BooleanSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderBooleanSetting(setting);
                    }
                }

                foreach (MultipleChoiceSetting setting in SettingsCollection.MultipleChoiceSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderMultipleChoiceSetting(setting);
                    }
                }

                foreach (PathSetting setting in SettingsCollection.PathSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderPathSetting(setting);
                    }
                }

                foreach (Setting<SettingsCollection> setting in SettingsCollection.SubSettings)
                {
                    if (!setting.Hidden)
                    {
                        RenderSubSetting(setting);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Error while loading settings:\n" + ex.Message);
                DialogResult = false;
                Close();
            }
        }

        private void RenderSubSetting(Setting<SettingsCollection> setting)
        {
            Controls.SubSetting settingControl = new Controls.SubSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void RenderMultipleChoiceSetting(MultipleChoiceSetting setting)
        {
            Controls.MultipleChoiceSetting settingControl = new Controls.MultipleChoiceSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void RenderBooleanSetting(Setting<bool> setting)
        {
            Controls.BooleanSetting settingControl = new Controls.BooleanSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void RenderPathSetting(PathSetting setting)
        {
            Controls.PathSetting settingControl = new Controls.PathSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void SubSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Setting<SettingsCollection> setting = (Setting<SettingsCollection>)(e.Source as System.Windows.Controls.Button).Tag;
            SettingsEditor editor = new SettingsEditor(setting.Value);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                setting.Value = editor.SettingsCollection;
            }
        }

        private void RenderTextSetting(Setting<string> setting)
        {
            Controls.TextSetting settingControl = new Controls.TextSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void RenderNumeralSetting(Setting<int> setting)
        {
            Controls.NumeralSetting settingControl = new Controls.NumeralSetting(setting);
            settingControl.MouseEnter += ShowDescription;
            setting.Control = settingControl;
            spControls.Children.Add(settingControl);
        }

        private void ShowDescription(object sender, MouseEventArgs e)
        {
            tbDescription.Text = (e.Source as FrameworkElement).Tag.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            foreach (Setting<int> setting in SettingsCollection.NumeralSettings)
            {
                SaveNumeralSetting(setting);
            }

            foreach (Setting<string> setting in SettingsCollection.TextSettings)
            {
                SaveTextSetting(setting);
            }

            foreach (Setting<bool> setting in SettingsCollection.BooleanSettings)
            {
                SaveBooleanSetting(setting);
            }

            foreach (MultipleChoiceSetting setting in SettingsCollection.MultipleChoiceSettings)
            {
                SaveMultipleChoiceSetting(setting);
            }

            foreach (PathSetting setting in SettingsCollection.PathSettings)
            {
                SavePathSetting(setting);
            }

            foreach (Setting<SettingsCollection> setting in SettingsCollection.SubSettings)
            {
                SaveSubSetting(setting);
            }

            if (valid)
            {
                DialogResult = true;
                Close();
            }
        }

        private void SaveSubSetting(Setting<SettingsCollection> setting)
        {
            Controls.SubSetting settingControl = setting.Control as Controls.SubSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }

        private void SavePathSetting(PathSetting setting)
        {
            Controls.PathSetting settingControl = setting.Control as Controls.PathSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }

        private void SaveMultipleChoiceSetting(MultipleChoiceSetting setting)
        {
            Controls.MultipleChoiceSetting settingControl = setting.Control as Controls.MultipleChoiceSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }

        private void SaveBooleanSetting(Setting<bool> setting)
        {
            Controls.BooleanSetting settingControl = setting.Control as Controls.BooleanSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }

        private void SaveTextSetting(Setting<string> setting)
        {
            Controls.TextSetting settingControl = setting.Control as Controls.TextSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }

        private void SaveNumeralSetting(Setting<int> setting)
        {
            Controls.NumeralSetting settingControl = setting.Control as Controls.NumeralSetting;
            if (!settingControl.Validate())
            {
                valid = false;
            }
            else
            {
                setting.Value = settingControl.GetValue();
            }
        }
    }
}
