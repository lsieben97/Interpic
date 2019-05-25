using HtmlAgilityPack;
using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Web.Tasks;
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
using System.Xml;

namespace Interpic.Web.Windows
{
    /// <summary>
    /// Interaction logic for SectionSelector.xaml
    /// </summary>
    public partial class NodeSelector : Window, ISectionIdentifierSelector, IControlIdentifierSelector
    {
        private List<HtmlNode> SearchResults;
        private int currentSearchResult;
        private HtmlNode baseNode;
        private Dictionary<HtmlNode, TreeViewItem> SearchIndex = new Dictionary<HtmlNode, TreeViewItem>();
        public NodeSelector(bool isGeneralNodeSelector = false)
        {
            InitializeComponent();
        }

        public HtmlNode SelectedNode { get; set; }
        public SectionIdentifier SectionIdentifier { get; set; }
        public ControlIdentifier ControlIdentifier { get; set; }
        public Models.Page Page { get; set; }
        public Models.Section Section { get; set; }

        public void LoadXmlNodes(HtmlNode baseNode)
        {
            TreeViewItem baseItem = GetTreeViewItem(baseNode);
            tvNodes.Items.Add(baseItem);
            this.baseNode = baseNode;
        }

        public void ShowSelector()
        {
            if (Page != null)
            {
                LoadXmlNodes(Page.GetWebPageExtensions().Document.DocumentNode);
            }
            else if (Section != null)
            {
                LoadXmlNodes(Section.Parent.GetWebPageExtensions().Document.DocumentNode.SelectSingleNode(Section.SectionIdentifier.Identifier));
            }
            ShowDialog();
        }

        private TreeViewItem GetTreeViewItem(HtmlNode currentNode)
        {
            TreeViewItem item = new TreeViewItem();
            item.Tag = currentNode;
            string name = Utils.GetFriendlyHtmlName(currentNode);

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // Label
            Label lbl = new Label();
            lbl.Content = name;

            item.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            item.FontSize = 14;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));


            // Add into stack
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;

            foreach (HtmlNode node in currentNode.ChildNodes)
            {
                item.Items.Add(GetTreeViewItem(node));
            }
            SearchIndex.Add(currentNode, item);
            return item;
        }

        private void tvNodes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedNode = ((TreeViewItem)((TreeView)sender).SelectedItem).Tag as HtmlNode;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNode == null)
            {
                ErrorAlert.Show("No node selected.\nPlease select a node.");
                return;
            }
            ControlIdentifier = new ControlIdentifier();
            ControlIdentifier.Name = Utils.GetFriendlyHtmlName(SelectedNode);
            ControlIdentifier.Identifier = SelectedNode.XPath;

            SectionIdentifier = new SectionIdentifier();
            SectionIdentifier.Name = Utils.GetFriendlyHtmlName(SelectedNode);
            SectionIdentifier.Identifier = SelectedNode.XPath;

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSearch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbSearchQuery.Text) == false && String.IsNullOrEmpty(tbSearchQuery.Text) == false)
            {
                SearchXmlNodeTask task = new SearchXmlNodeTask(tbSearchQuery.Text, baseNode, new string[] { "id", "class" });
                ProcessTaskDialog dialog = new ProcessTaskDialog(task);
                dialog.ShowDialog();
                SearchResults = task.SearchResults;
                if (SearchResults.Count > 0)
                {
                    if (SearchResults.Count > 1)
                    {
                        btnPreviousSearchResult.Visibility = Visibility.Visible;
                        btnNextSearchResult.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnPreviousSearchResult.Visibility = Visibility.Hidden;
                        btnNextSearchResult.Visibility = Visibility.Hidden;
                    }
                    currentSearchResult = 0;
                    lbSearchStatus.Text = string.Format("Found {0} result. Showing result {1}", SearchResults.Count, currentSearchResult + 1);
                    UpdateCurrentSearchResult();
                }
                else
                {
                    btnPreviousSearchResult.Visibility = Visibility.Hidden;
                    btnNextSearchResult.Visibility = Visibility.Hidden;
                    lbSearchStatus.Text = "Search returned no results.";
                }
            }
        }

        private void UpdateCurrentSearchResult()
        {
            lbSearchStatus.Text = string.Format("Found {0} result. Showing result {1}", SearchResults.Count, currentSearchResult + 1);
            TreeViewItem item = SearchIndex[SearchResults[currentSearchResult]];
            item.BringIntoView();
            item.IsSelected = true;
        }

        private void btnPreviousSearchResult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentSearchResult == 0)
            {
                currentSearchResult = SearchResults.Count - 1;
            }
            else
            {
                currentSearchResult--;
            }
            UpdateCurrentSearchResult();
        }

        private void btnNextSearchResult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentSearchResult == SearchResults.Count - 1)
            {
                currentSearchResult = 0;
            }
            else
            {
                currentSearchResult++;
            }
            UpdateCurrentSearchResult();
        }
    }
}
