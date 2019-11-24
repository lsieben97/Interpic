using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interpic.UI.Controls
{
    public class StudioTabItem : TabItem, IStudioTab
    {

        public bool DoesContainChanges { get; set; } = false;
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(StudioTabItem), new PropertyMetadata(""));


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public bool ForceClose { get; set; }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(StudioTabItem), new PropertyMetadata(null));

        public static readonly RoutedEvent CloseTabEvent = EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StudioTabItem));

        public event RoutedEventHandler CloseTab
        {
            add { AddHandler(CloseTabEvent, value); }
            remove { RemoveHandler(CloseTabEvent, value); }
        }

        static StudioTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StudioTabItem), new System.Windows.FrameworkPropertyMetadata(typeof(StudioTabItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button closeButton = GetTemplateChild("closeButton") as Button;
            TextBlock titleTextBlock = GetTemplateChild("title") as TextBlock;
            Image iconImage = GetTemplateChild("icon") as Image;

            closeButton.Click += new RoutedEventHandler(CloseButtonClicked);
            titleTextBlock.Text = Title;
            iconImage.Source = Icon;   
        }


        public void SetStudioView(string title, ImageSource icon, Control content)
        {
            Content = new StudioViewHolder(title, icon, content);
            
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseTabEvent, this));
        }

        public void ContainsChanges(bool changes)
        {
            TextBlock titleTextBlock = GetTemplateChild("title") as TextBlock;
            titleTextBlock.Text = changes ? Title + "*" : Title;
            DoesContainChanges = changes;
        }

        public void SetTitle(string title)
        {
            Title = title;
            TextBlock titleTextBlock = GetTemplateChild("title") as TextBlock;
            titleTextBlock.Text = DoesContainChanges ? Title + "*" : Title;
        }

        void IStudioTab.CloseTab()
        {
            RaiseEvent(new RoutedEventArgs(CloseTabEvent, this));
        }

        void IStudioTab.Focus()
        {
            TabControl parent = Parent as TabControl;
            if (parent != null)
            {
                parent.SelectedItem = this;
            }
            
        }
    }
}
