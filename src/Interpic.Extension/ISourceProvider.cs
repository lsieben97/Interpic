using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Extensions
{
    public interface ISourceProvider
    {
        /// <summary>
        /// Get a source for the given page of the given project.
        /// </summary>
        /// <param name="project">The current project.</param>
        /// <param name="page">The page to get the source from.</param>
        /// <returns>The page source.</returns>
        string GetSource(ref Project project, ref Page page);
    }
}
