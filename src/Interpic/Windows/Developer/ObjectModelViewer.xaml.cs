using Interpic.Studio.Functional;
using Interpic.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Interpic.Studio.Windows.Developer
{
    /// <summary>
    /// Interaction logic for ObjectModelViewer.xaml
    /// </summary>
    public partial class ObjectModelViewer : Window
    {
        private object viewingObject;
        public ObjectModelViewer(object obj)
        {
            InitializeComponent();
            viewingObject = obj;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JToken token = JToken.Parse(JsonConvert.SerializeObject(viewingObject));
            List<JToken> children = new List<JToken>();
            if (token != null)
            {
                children.Add(token);
            }

            tvObjectTree.ItemsSource = null;
            tvObjectTree.Items.Clear();
            tvObjectTree.ItemsSource = children;
        }
    }
}
