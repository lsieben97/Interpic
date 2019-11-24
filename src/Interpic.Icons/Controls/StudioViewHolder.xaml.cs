using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interpic.UI.Controls
{
    /// <summary>
    /// Interaction logic for StudioViewHolder.xaml
    /// </summary>
    public partial class StudioViewHolder : UserControl
    {
        public StudioViewHolder(string title, ImageSource icon, Control content)
        {
            InitializeComponent();
            imViewIcon.Source = icon;
            lbViewTitle.Text = title;
            gViewContent.Children.Add(content);
            SizeChanged += StudioViewHolder_SizeChanged;
        }

        private void StudioViewHolder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeContent();
        }

        private void ResizeContent()
        {
            FrameworkElement parent = Parent as FrameworkElement;
            FrameworkElement parentOfParent = parent.Parent as FrameworkElement;
            gViewContent.Height = parentOfParent.ActualHeight - parent.ActualHeight - 63;
            gViewContent.Width = parentOfParent.ActualWidth;
        }

        public override void OnApplyTemplate()
        {
            ResizeContent();
        }
    }
}
