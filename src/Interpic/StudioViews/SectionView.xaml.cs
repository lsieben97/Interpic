using Interpic.Models;
using Interpic.Studio.Controls;
using Interpic.Studio.Windows.Behaviours;
using Interpic.UI.Controls;
using Interpic.Utils;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for SectionView.xaml
    /// </summary>
    public partial class SectionView : UserControl, IStudioViewHandler
    {
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        public Models.Section Section { get; }

        public SectionView(Models.Section section)
        {
            InitializeComponent();
            Section = section;
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.ShowManualItemSettings(Section);
        }

        private void BtnGetManualSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.LoadManualItem(Section);
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Studio.RemoveManualItem(Section, true);
        }

        public void ViewAttached() { }

        public void ViewDetached()
        {
            UnBindEvents();
        }

        private void LoadControls()
        {
            spControls.Children.Clear();
            foreach (Models.Control control in Section.Controls)
            {
                ControlListItem pageListItem = new ControlListItem(control, Studio);
                spControls.Children.Add(pageListItem);
            }
        }

        private void BindEvents()
        {
            Section.Controls.CollectionChanged += ControlCollectionChanged;
            Section.PropertyChanged += SectionPropertyChanged;
        }

        private void SectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description")
            {
                if (StudioTab.DoesContainChanges == false)
                {
                    tbSectionDescription.Text = Section.Description;
                }
            }
            else if (e.PropertyName == "IsLoaded")
            {
                btnRefresh.Visibility = Section.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void UnBindEvents()
        {
            Section.Controls.CollectionChanged -= ControlCollectionChanged;
            Section.PropertyChanged -= SectionPropertyChanged;
        }

        private void ControlCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LoadControls();
        }

        public string GetTabContents()
        {
            return Section.Id;
        }

        private void TbSectionDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            StudioTab.ContainsChanges(true);
        }

        private void BtnSaveSectionDescription_Click(object sender, RoutedEventArgs e)
        {
            StudioTab.ContainsChanges(false);
            Section.Description = tbSectionDescription.Text;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StudioTab.SetTitle(Section.Name);
            tbSectionDescription.Text = Section.Description;
            LoadControls();
            BindEvents();
            btnRefresh.Visibility = Section.IsLoaded ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BtnAddControl_Click(object sender, RoutedEventArgs e)
        {
            Studio.AddControl(Section, true);
        }

        private void btnSelectBehaviours_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PickBehaviours pickBehavioursWindow = new PickBehaviours(Studio.GetBehaviourConfiguration().Behaviours, Section.Behaviours);
            pickBehavioursWindow.ShowDialog();
            Section.Behaviours = pickBehavioursWindow.SelectedBehaviours;
            Section.BehaviourIds = BehaviourUtils.GetBehaviourIds(Section.Behaviours);
        }
    }
}
