using System.Collections.ObjectModel;

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
