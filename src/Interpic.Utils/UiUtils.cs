using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interpic.Utils
{
    public static class UiUtils
    {
        // from https://stackoverflow.com/a/13642340
        public static TreeViewItem GetTreeViewItem(string text, string imagePath)
        {
            TreeViewItem item = new TreeViewItem();
            item.IsExpanded = false;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create Image
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/" + imagePath, UriKind.RelativeOrAbsolute));
            image.Width = 24;
            image.Height = 24;
            // Label
            Label lbl = new Label();
            lbl.Content = text;

            item.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            item.FontSize = 14;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));


            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;
            return item;
        }
    }
}
