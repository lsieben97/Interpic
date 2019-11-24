using Interpic.Models;
using Interpic.Studio.Controls;
using Interpic.UI.Controls;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for PageView.xaml
    /// </summary>
    public partial class PageView : UserControl, IStudioViewHandler
    {
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        public Models.Page Page { get; set; }

        public PageView(Models.Page page)
        {
            InitializeComponent();
            Page = page;
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.ShowManualItemSettings(Page);
        }

        private void BtnGetManualSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (Models.Page page, bool succes) result = Studio.LoadManualItem(Page);
            if (result.succes)
            {
                Page = result.page;
            }
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.RemoveManualItem(Page, true);
        }

        public void ViewAttached() {  }

        public void ViewDetached()
        {
            UnBindEvents();
        }

        public string GetTabContents()
        {
            return Page.Id;
        }

        private void LoadSections()
        {
            spSections.Children.Clear();
            foreach (Models.Section section in Page.Sections)
            {
                SectionListItem pageListItem = new SectionListItem(section, Studio);
                spSections.Children.Add(pageListItem);
            }
        }

        private void BindEvents()
        {
            Page.Sections.CollectionChanged += SectionsCollectionChanged;
            Page.PropertyChanged += PagePropertyChanged;
        }

        private void PagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description")
            {
                if (StudioTab.DoesContainChanges == false)
                {
                    tbPageDescription.Text = Page.Description;
                }
            }
            else if (e.PropertyName == "IsLoaded")
            {
                btnGetManualSource.Visibility = Page.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void SectionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LoadSections();
        }

        private void UnBindEvents()
        {
            Page.Sections.CollectionChanged -= SectionsCollectionChanged;
            Page.PropertyChanged -= PagePropertyChanged;
        }

        private void BtnSavePageDescription_Click(object sender, RoutedEventArgs e)
        {
            StudioTab.ContainsChanges(false);
            Page.Description = tbPageDescription.Text;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StudioTab.SetTitle(Page.Name);
            
            tbPageDescription.Text = Page.Description;
            LoadSections();
            BindEvents();
            btnGetManualSource.Visibility = Page.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
            btnAddSection.Visibility = Page.Type == Models.Page.PAGE_TYPE_TEXT ? Visibility.Collapsed : Visibility.Visible;
            if (Page.Type == Models.Page.PAGE_TYPE_TEXT)
            {
                grid.RowDefinitions[3].Height = new GridLength(0);
                grid.RowDefinitions[4].Height = new GridLength(0);
            }
        }

        private void TbPageDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            StudioTab.ContainsChanges(true);
        }

        private void BtnAddSection_Click(object sender, RoutedEventArgs e)
        {
            Studio.AddSection(Page, true);
        }
    }
}
