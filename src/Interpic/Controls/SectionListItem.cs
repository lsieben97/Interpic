using Interpic.Alerts;
using Interpic.Models;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Interpic.Studio.Controls
{
    /// <summary>
    /// Interaction logic for PageListItem.xaml
    /// </summary>
    public partial class SectionListItem : UserControl
    {
        private readonly IStudioEnvironment studio;

        public SectionListItem(Models.Section section, IStudioEnvironment studio)
        {
            InitializeComponent();
            Section = section;
            this.studio = studio;
            lbSectionName.Text = section.Name;
            Section.PropertyChanged += SectionPropertyChanged;
            btnRefresh.Visibility = Section.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void SectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoaded")
            {
                btnRefresh.Visibility = Section.IsLoaded ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }

        public Models.Section Section { get; }

        private void BtnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Section);
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowManualItemSettings(Section);
        }

        private void BtnRefresh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.LoadManualItem(Section);
        }

        private void BtnRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.RemoveManualItem(Section, true);
        }

        private void LbPageName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            studio.ShowBuiltinStudioView(Section);
        }
    }
}
