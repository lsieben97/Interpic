using Interpic.Web.Behaviours.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        }

        public ElementSelectorEditor(ElementSelector selector) : this()
        {
            Selector = selector;
            cbbSelectorType.SelectedValue = Selector.SelectorType;
            tbSelector.Text = Selector.Selector;
        }

        private void CbbSelectorType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ElementSelectorType)cbbSelectorType.SelectedValue) == ElementSelectorType.XPath)
            {
                tbSelector.Width = 492;
                btnSelect.Visibility = Visibility.Visible;
            }
            else
            {
                tbSelector.Width = double.NaN;
                btnSelect.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Selector = null;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
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
