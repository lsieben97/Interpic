namespace Interpic.Extensions
{
    public enum ExtensionType
    {
        /// <summary>
        /// Defines a new project type using the <see cref="IProjectTypeProvider"/>.
        /// </summary>
        ProjectType,

        /// <summary>
        /// Defines a new output type using the  <see cref="IProjectBuilder"/>.
        /// </summary>
        Builder,

        SourceProvider
    }
}