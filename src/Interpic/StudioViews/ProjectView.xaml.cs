using Interpic.Models;
using Interpic.Studio.Controls;
using Interpic.UI.Controls;
using System;
using System.Windows.Controls;


namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl, IStudioViewHandler
    {
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        public Models.Version Version { get; }

        public ProjectView(Models.Version version)
        {
            InitializeComponent();
            Version = version;
        }

        public string GetTabContents()
        {
            return Version.Id;
        }

        public void ViewAttached()
        {
            LoadPages();
            BindEvents();
        }

        private void BindEvents()
        {
            Version.Pages.CollectionChanged += PagesCollectionChanged;
        }

        private void UnBindEvents()
        {
            Version.Pages.CollectionChanged -= PagesCollectionChanged;
        }

        private void PagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                LoadPages();
            }
        }

        private void LoadPages()
        {
            spPages.Children.Clear();
            foreach (Models.Page page in Version.Pages)
            {
                PageListItem pageListItem = new PageListItem(page, Studio);
                spPages.Children.Add(pageListItem);
            }
        }

        public void ViewDetached()
        {
            UnBindEvents();
        }

        private void ShowSettings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Studio.ShowManualItemSettings(Version);
        }

        private void BtnAddPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Studio.AddPage(Version, true);
        }
    }
}
