using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Web.Tasks
{
    public class SearchXmlNodeTask : AsyncTasks.AsyncTask
    {
        public string SearchQuery { get; set; }
        public HtmlNode SearchTarget { get; set; }
        public List<HtmlNode> SearchResults { get; set; }
        public string[] AttributesToCheck { get; set; }

        public SearchXmlNodeTask(string searchQuery, HtmlNode searchTarget, string[] attributesToCheck = null)
        {
            TaskName = "Searching...";
            TaskDescription = searchQuery;
            ActionName = "Search in nodes";
            IsIndeterminate = true;

            SearchQuery = searchQuery;
            SearchTarget = searchTarget;
            AttributesToCheck = attributesToCheck;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            SearchResults = new List<HtmlNode>();
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
            if (node.Name == SearchQuery)
            {
                SearchResults.Add(node);
            }
            if (AttributesToCheck != null)
            {
                if (node.Attributes != null)
                {
                    foreach (string attributeToCheck in AttributesToCheck)
                    {
                        HtmlAttribute attribute = node.Attributes.ToList().Find((attr) => attr.Name == attributeToCheck);
                        if (attribute != null)
                        {
                            if (attribute.Value == SearchQuery)
                            {
                                SearchResults.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}
