using Interpic.Web.Behaviours.Models;
using System;
using System.Linq;
using System.Windows;
using static Interpic.Web.Behaviours.Models.ElementSelector;

namespace Interpic.Web.Behaviours.Windows
{
    /// <summary>
    /// Interaction logic for ElementSelectorEditor.xaml
    /// </summary>
    public partial class ElementSelectorEditor : Window
    {
        public ElementSelector Selector { get; set; }
        public ElementSelectorEditor()
        {
            InitializeComponent();
            cbbSelectorType.ItemsSource = Enum.GetValues(typeof(ElementSelector.ElementSelectorType)).Cast<ElementSelector.ElementSelectorType>();
            cbbSelectorType.SelectedIndex = 0;
        }

        public ElementSelectorEditor(ElementSelector selector) : this()
        {
            Selector = selector;
            cbbSelectorType.SelectedValue = Selector.SelectorType;
            tbSelector.Text = Selector.Selector;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Selector = null;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSelector.Text))
            {
                lbSelectorError.Text = "Selector is required";
                return;
            }

            if (Selector == null)
            {
                Selector = new ElementSelector();
            }

            
            Selector.SelectorType = (ElementSelectorType)cbbSelectorType.SelectedValue;
            Selector.Selector = tbSelector.Text;
            Close();
        }
    }
}
