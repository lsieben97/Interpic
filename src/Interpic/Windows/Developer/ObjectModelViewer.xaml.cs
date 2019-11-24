using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;

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
