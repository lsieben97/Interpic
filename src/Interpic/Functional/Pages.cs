using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Settings;
using Interpic.Studio.Windows;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interpic.Studio.Functional
{
    public static class Pages
    {
        internal static Studio Studio { get; }
         public static UIElement RenderTextPage(Models.Page page)
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Height = double.NaN;

            // title
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            titleTextBlock.FontSize = 24;
            titleTextBlock.Width = double.NaN;
            titleTextBlock.Margin = new Thickness(12, 12, 0, 0);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            titleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            titleTextBlock.Text = page.Name;
            grid.Children.Add(titleTextBlock);

            // stack panel for edit and remove buttons
            StackPanel panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Right;
            panel.VerticalAlignment = VerticalAlignment.Top;
            panel.Margin = new Thickness(0, 12, 12, 0);
            panel.Orientation = Orientation.Horizontal;
            panel.Width = double.NaN;
            panel.Height = double.NaN;
            grid.Children.Add(panel);

            // remove button
            Button removeButton = new Button();
            removeButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            removeButton.Content = "Remove";
            removeButton.Tag = page;
            removeButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            removeButton.HorizontalAlignment = HorizontalAlignment.Right;
            removeButton.VerticalAlignment = VerticalAlignment.Top;
            removeButton.Margin = new Thickness(0, 0, 12, 0);
            removeButton.Click += RemoveButton_Click;
            panel.Children.Add(removeButton);

            // edit button
            Button editButton = new Button();
            editButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            editButton.Content = "Edit";
            editButton.Tag = page;
            editButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            editButton.HorizontalAlignment = HorizontalAlignment.Right;
            editButton.VerticalAlignment = VerticalAlignment.Top;
            editButton.Margin = new Thickness(0, 0, 0, 0);
            editButton.Click += EditButton_Click;
            panel.Children.Add(editButton);
            

            // text area
            TextBox editArea = new TextBox();
            editArea.Margin = new Thickness(12,48 , 12, 48);
            editArea.HorizontalAlignment = HorizontalAlignment.Stretch;
            editArea.VerticalAlignment = VerticalAlignment.Stretch;
            editArea.Height = 450;
            editArea.AcceptsReturn = true;
            editArea.AcceptsTab = true;
            editArea.Tag = page;
            editArea.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            editArea.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            editArea.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            editArea.TextChanged += EditArea_TextChanged;
            grid.Children.Add(editArea);

            return grid;
        }

        private static void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Page page = ((FrameworkElement)e.Source).Tag as Models.Page;
            ConfirmAlert alert = ConfirmAlert.Show("'" + page.Name + "' will be removed.");
            if (alert.Result == true)
            {
                Studio.RemovePage(page);
            }
        }

        private static void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Page page = ((FrameworkElement)e.Source).Tag as Models.Page;
            NewPage editPage = new NewPage(page);
            editPage.ShowDialog();
            Studio.RedrawTreeView();
            
            Studio.ReselectPage(page);
        }

        private static void EditArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            Models.Page page = ((FrameworkElement)e.Source).Tag as Models.Page;
            page.Description = (e.Source as TextBox).Text;
        }
    }
}