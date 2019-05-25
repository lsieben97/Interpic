using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Models.Extensions
{
    public interface IControlFinder
    {
        /// <summary>
        /// Find control nodes in the given node.
        /// </summary>
        /// <param name="node">The node to search in.</param>
        /// <returns>Control nodes in the given node.</returns>
        ObservableCollection<DiscoveredControl> FindControls(Section section);
    }
}
