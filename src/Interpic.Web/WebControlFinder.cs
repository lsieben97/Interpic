using HtmlAgilityPack;
using Interpic.AsyncTasks;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Web.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Web
{
    public class WebControlFinder : IControlFinder
    {
        public ObservableCollection<DiscoveredControl> FindControls(Section section)
        {
            FindWebControlsTask task = new FindWebControlsTask(section.Parent.GetWebPageExtensions().Document.DocumentNode);
            new ProcessTaskDialog(task).ShowDialog();
            ObservableCollection<DiscoveredControl> results = new ObservableCollection<DiscoveredControl>();
            foreach (HtmlNode node in task.SearchResults)
            {
                DiscoveredControl discoveredControl = new DiscoveredControl();
                discoveredControl.Name = Utils.GetFriendlyHtmlName(node);
                discoveredControl.Identifier = new ControlIdentifier();
                discoveredControl.Identifier.Identifier = node.XPath;
                discoveredControl.Identifier.Name = Utils.GetFriendlyHtmlName(node);
                results.Add(discoveredControl);
            }
            return results;
        }
    }
}
