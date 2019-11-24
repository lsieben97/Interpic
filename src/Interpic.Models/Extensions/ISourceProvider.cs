namespace Interpic.Models.Extensions
{
    public interface ISourceProvider
    {
        /// <summary>
        /// Get a source for the given page of the given project.
        /// </summary>
        /// <param name="project">The current project.</param>
        /// <param name="page">The page to get the source from.</param>
        /// <returns>The page source.</returns>
        Page GetSource(Page page);
    }
}
