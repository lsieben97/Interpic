using Interpic.Alerts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 18;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(7, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(3, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            Button openSubSettingsButton = new Button();
            openSubSettingsButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            openSubSettingsButton.Content = "Open " + setting.Name;
            openSubSettingsButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            openSubSettingsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            openSubSettingsButton.Margin = new Thickness(3, 0, 3, 0);
            openSubSettingsButton.Width = double.NaN;
            openSubSettingsButton.VerticalAlignment = VerticalAlignment.Top;


            openSubSettingsButton.Click += SubSettingsButton_Click;
            openSubSettingsButton.Tag = setting;

            spControls.Children.Add(openSubSettingsButton);
            RenderSeperator();
        }

        private void RenderMultipleChoiceSetting(MultipleChoiceSetting setting)
        {
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 14;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(3, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            // text box
            ComboBox comboBox = new ComboBox();
            comboBox.Style = Application.Current.Resources["ComboBoxStyle"] as Style;
            comboBox.VerticalAlignment = VerticalAlignment.Top;
            comboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            comboBox.Margin = new Thickness(6, 0, 6, 3);
            comboBox.Width = double.NaN;
            comboBox.Tag = setting.Description;
            comboBox.MouseEnter += SettingTitleTextBlock_MouseEnter;

            foreach (KeyValuePair<string, string> choice in setting.Choices)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = choice.Key;
                item.Tag = choice.Value;
                comboBox.Items.Add(item);
            }
            comboBox.SelectedItem = GetComboBoxItemForValue(comboBox, setting.Value);
            spControls.Children.Add(comboBox);
            setting.Control = comboBox;
            RenderSeperator();
        }

        private void RenderBooleanSetting(Setting<bool> setting)
        {
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 14;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            // text box
            CheckBox checkBox = new CheckBox();
            checkBox.Style = Application.Current.Resources["CheckBoxStyle"] as Style;
            checkBox.VerticalAlignment = VerticalAlignment.Top;
            checkBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            checkBox.Width = double.NaN;
            checkBox.Margin = new Thickness(6, 0, 6, 3);
            checkBox.Tag = setting.Description;
            checkBox.MouseEnter += SettingTitleTextBlock_MouseEnter;
            spControls.Children.Add(checkBox);

            checkBox.IsChecked = setting.Value;
            setting.Control = checkBox;

            RenderSeperator();
        }

        private void RenderPathSetting(PathSetting setting)
        {
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 14;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            // sizes
            double containerWidthMinusDefaultPadding = spControls.ActualWidth - 15; // 6 on sides + 3 between button and textbox
            double buttonWidth = (containerWidthMinusDefaultPadding / 10) * 3;
            double textboxWidth = (containerWidthMinusDefaultPadding / 10) * 7;

            StackPanel controlPanel = new StackPanel();
            controlPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            controlPanel.Margin = new Thickness(6, 0, 6, 3);
            controlPanel.Orientation = Orientation.Horizontal;
            controlPanel.Height = double.NaN;
            controlPanel.Width = double.NaN;
            spControls.Children.Add(controlPanel);

            // text box
            TextBox textBox = new TextBox();
            textBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.MouseEnter += SettingTitleTextBlock_MouseEnter;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.Width = textboxWidth;
            textBox.IsReadOnly = true;
            textBox.Margin = new Thickness(0, 0, 3, 0);
            textBox.Tag = setting.Description;
            controlPanel.Children.Add(textBox);

            // select path button
            Button selectButton = new Button();
            selectButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            selectButton.Content = "Select";
            selectButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            selectButton.HorizontalAlignment = HorizontalAlignment.Left;
            selectButton.VerticalAlignment = VerticalAlignment.Top;
            selectButton.Width = buttonWidth;
            selectButton.Click += PathButton_Click;
            selectButton.Tag = setting;
            controlPanel.Children.Add(selectButton);

            textBox.Text = setting.Value.ToString();

            setting.Control = textBox;

            RenderSeperator();
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

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            PathSetting setting = (PathSetting)(e.Source as System.Windows.Controls.Button).Tag;
            if (setting.Type == PathSetting.PathType.Folder)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (setting.StartPath != null)
                {
                    dialog.SelectedPath = setting.StartPath;
                }
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TextBox control = setting.Control as TextBox;
                    control.Text = dialog.SelectedPath;
                }
            }
            else if (setting.Type == PathSetting.PathType.File)
            {
                if (setting.Operation == PathSetting.PathOperation.Save)
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
                            TextBox control = setting.Control as TextBox;
                            control.Text = dialog.FileName;
                        }
                    }
                }
                else if (setting.Operation == PathSetting.PathOperation.Load)
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
                            TextBox control = setting.Control as TextBox;
                            control.Text = dialog.FileName;
                        }
                    }
                }
            }
        }

        private void RenderTextSetting(Setting<string> setting)
        {
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 14;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            // text box
            TextBox textBox = new TextBox();
            textBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.MouseEnter += SettingTitleTextBlock_MouseEnter;
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.Width = double.NaN;
            textBox.Margin = new Thickness(6, 0, 6, 3);
            textBox.Tag = setting.Description;
            textBox.Text = setting.Value.ToString();
            spControls.Children.Add(textBox);

            setting.Control = textBox;

            RenderSeperator();
        }

        private void RenderNumeralSetting(Setting<int> setting)
        {
            // title / error message stack panel
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 3);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Height = double.NaN;
            titlePanel.Width = double.NaN;

            // title
            TextBlock settingTitleTextBlock = new TextBlock();
            settingTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingTitleTextBlock.FontSize = 14;
            settingTitleTextBlock.Width = double.NaN;
            settingTitleTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingTitleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            settingTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            settingTitleTextBlock.Text = setting.Name;
            settingTitleTextBlock.Tag = setting.Description;
            settingTitleTextBlock.MouseEnter += SettingTitleTextBlock_MouseEnter;
            titlePanel.Children.Add(settingTitleTextBlock);

            // error message
            TextBlock settingErrorTextBlock = new TextBlock();
            settingErrorTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            settingErrorTextBlock.FontSize = 9;
            settingErrorTextBlock.Width = double.NaN;
            settingErrorTextBlock.Margin = new Thickness(6, 0, 0, 0);
            settingErrorTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            settingErrorTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            settingErrorTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            settingErrorTextBlock.Text = String.Empty;
            setting.InvalidLabel = settingErrorTextBlock;
            titlePanel.Children.Add(settingErrorTextBlock);

            spControls.Children.Add(titlePanel);

            // number picker
            TextBox numberBox = new TextBox();
            numberBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            numberBox.VerticalAlignment = VerticalAlignment.Top;
            numberBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            numberBox.Width = double.NaN;
            numberBox.Margin = new Thickness(6, 0, 6, 3);
            numberBox.Tag = setting.Description;
            numberBox.MouseEnter += SettingTitleTextBlock_MouseEnter;
            numberBox.PreviewTextInput += NumberValidationTextBox;

            numberBox.Text = setting.Value.ToString();

            spControls.Children.Add(numberBox);

            setting.Control = numberBox;
            RenderSeperator();
        }

        private ComboBoxItem GetComboBoxItemForValue(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag.ToString() == value)
                {
                    return item;
                }
            }
            return null;
        }
        private void RenderSeperator()
        {
            // line between controls
            Separator numberSeperator = new Separator();
            numberSeperator.HorizontalAlignment = HorizontalAlignment.Stretch;
            numberSeperator.Width = double.NaN;
            numberSeperator.Height = double.NaN;
            numberSeperator.Margin = new Thickness(6, 2, 6, 2);
            spControls.Children.Add(numberSeperator);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SettingTitleTextBlock_MouseEnter(object sender, MouseEventArgs e)
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

            if (valid)
            {
                DialogResult = true;
                Close();
            }
        }

        private void SavePathSetting(PathSetting setting)
        {
            string value = (setting.Control as TextBox).Text;
            SettingValidationResult result = setting.Validator.Validate(value);
            if (!result.IsValid)
            {
                setting.InvalidLabel.Text = result.ErrorMessage;
                valid = false;
                return;
            }
            setting.Value = value;
        }

        private void SaveMultipleChoiceSetting(MultipleChoiceSetting setting)
        {
            string value = ((ComboBoxItem)(setting.Control as ComboBox).SelectedItem).Tag.ToString();
            SettingValidationResult result = setting.Validator.Validate(value);
            if (!result.IsValid)
            {
                setting.InvalidLabel.Text = result.ErrorMessage;
                valid = false;
                return;
            }
            setting.Value = value;
        }

        private void SaveBooleanSetting(Setting<bool> setting)
        {
            bool? value = (setting.Control as CheckBox).IsChecked;
            if (!value.HasValue)
            {
                valid = false;
                return;
            }
            SettingValidationResult result = setting.Validator.Validate(value.Value);
            if (!result.IsValid)
            {
                setting.InvalidLabel.Text = result.ErrorMessage;
                valid = false;
                return;
            }
            setting.Value = value.Value;
        }

        private void SaveTextSetting(Setting<string> setting)
        {
            string value = (setting.Control as TextBox).Text;
            SettingValidationResult result = setting.Validator.Validate(value);
            if (!result.IsValid)
            {
                setting.InvalidLabel.Text = result.ErrorMessage;
                valid = false;
                return;
            }
            setting.Value = value;
        }

        private void SaveNumeralSetting(Setting<int> setting)
        {
            int value = Convert.ToInt32((setting.Control as TextBox).Text);
            SettingValidationResult result = setting.Validator.Validate(value);
            if (!result.IsValid)
            {
                setting.InvalidLabel.Text = result.ErrorMessage;
                valid = false;
                return;
            }
            setting.Value = value;
        }
    }
}
