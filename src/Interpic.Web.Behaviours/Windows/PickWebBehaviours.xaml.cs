using Interpic.Web.Behaviours.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for PickWebBehaviours.xaml
    /// </summary>
    public partial class PickWebBehaviours : Window
    {
        private readonly ObservableCollection<WebBehaviour> availableBehaviours;

        public ObservableCollection<WebBehaviour> SelectedBehaviours { get; set; }

        public PickWebBehaviours(List<WebBehaviour> availableBehaviours, List<WebBehaviour> selectedBehaviours)
        {
            InitializeComponent();
            this.availableBehaviours = new ObservableCollection<WebBehaviour>(availableBehaviours);
            SelectedBehaviours = new ObservableCollection<WebBehaviour>(selectedBehaviours);
            lsbSelectedBehaviours.ItemsSource = SelectedBehaviours;
            lsbAvailableBehaviours.ItemsSource = this.availableBehaviours;
        }

        public PickWebBehaviours(List<WebBehaviour> availableBehaviours)
        {
            InitializeComponent();
            this.availableBehaviours = new ObservableCollection<WebBehaviour>(availableBehaviours);
            SelectedBehaviours = new ObservableCollection<WebBehaviour>();
            lsbSelectedBehaviours.ItemsSource = SelectedBehaviours;
            lsbAvailableBehaviours.ItemsSource = this.availableBehaviours;
        }

        private void LsbAvailableBehaviours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lsbSelectedBehaviours.SelectedIndex = -1;
            btnAddToSelected.IsEnabled = lsbAvailableBehaviours.SelectedValue != null;
            btnRemoveFromSelected.IsEnabled = lsbSelectedBehaviours.SelectedValue != null;
        }

        private void LsbSelectedBehaviours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lsbAvailableBehaviours.SelectedIndex = -1;
            btnAddToSelected.IsEnabled = lsbAvailableBehaviours.SelectedValue != null;
            btnRemoveFromSelected.IsEnabled = lsbSelectedBehaviours.SelectedValue != null;
        }

        private void BtnAddToSelected_Click(object sender, RoutedEventArgs e)
        {
            WebBehaviour selectedbehaviour = (WebBehaviour)lsbAvailableBehaviours.SelectedValue;
            SelectedBehaviours.Add(selectedbehaviour);
            availableBehaviours.Remove(selectedbehaviour);
        }

        private void BtnRemoveFromSelected_Click(object sender, RoutedEventArgs e)
        {
            WebBehaviour selectedbehaviour = (WebBehaviour)lsbSelectedBehaviours.SelectedValue;
            SelectedBehaviours.Remove(selectedbehaviour);
            availableBehaviours.Add(selectedbehaviour);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedBehaviours = null;
            Close();
        }
    }
}
