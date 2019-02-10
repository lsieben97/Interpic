using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace Interpic.Web.Tasks
{
    public class FindWebControlsTask : AsyncTask
    {
        public HtmlNode SearchTarget { get; set; }
        public ObservableCollection<HtmlNode> SearchResults { get; set; }

        public FindWebControlsTask(HtmlNode searchTarget)
        {
            TaskName = "Searching for webcontrols...";
            TaskDescription = "";
            ActionName = "Search for webcontrols";
            IsIndeterminate = true;
            SearchTarget = searchTarget;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            SearchResults = new ObservableCollection<HtmlNode>();
            Stack<HtmlNode> nodes = new Stack<HtmlNode>();
            nodes.Push(SearchTarget);

            while (nodes.Count > 0)
            {
                HtmlNode current = nodes.Pop();

                foreach (HtmlNode n in current.ChildNodes)
                {
                    nodes.Push(n);
                }

                SearchNode(current);
            }
        }

        private void SearchNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element && (node.Name == "input" || node.Name == "select" || node.Name == "textarea"))
            {
                SearchResults.Add(node);
            }
        }
    }
}
