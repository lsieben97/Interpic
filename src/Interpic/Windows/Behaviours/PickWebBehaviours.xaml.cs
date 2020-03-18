using Interpic.Models.Behaviours;
using Interpic.Web.Behaviours.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows.Behaviours
{
    /// <summary>
    /// Interaction logic for PickWebBehaviours.xaml
    /// </summary>
    public partial class PickBehaviours : Window
    {
        private readonly ObservableCollection<Behaviour> availableBehaviours;

        public ObservableCollection<Behaviour> SelectedBehaviours { get; set; }

        public PickBehaviours(List<Behaviour> availableBehaviours, ObservableCollection<Behaviour> selectedBehaviours)
        {
            InitializeComponent();
            this.availableBehaviours = new ObservableCollection<Behaviour>(availableBehaviours);
            SelectedBehaviours = selectedBehaviours;
            lsbSelectedBehaviours.ItemsSource = SelectedBehaviours;
            lsbAvailableBehaviours.ItemsSource = this.availableBehaviours;
        }

        public PickBehaviours(List<Behaviour> availableBehaviours)
        {
            InitializeComponent();
            this.availableBehaviours = new ObservableCollection<Behaviour>(availableBehaviours);
            SelectedBehaviours = new ObservableCollection<Behaviour>();
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
            Behaviour selectedbehaviour = (Behaviour)lsbAvailableBehaviours.SelectedValue;
            SelectedBehaviours.Add(selectedbehaviour);
            availableBehaviours.Remove(selectedbehaviour);
        }

        private void BtnRemoveFromSelected_Click(object sender, RoutedEventArgs e)
        {
            Behaviour selectedbehaviour = (Behaviour)lsbSelectedBehaviours.SelectedValue;
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
