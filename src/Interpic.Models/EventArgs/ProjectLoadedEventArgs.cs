namespace Interpic.Models.EventArgs
{
    public class ProjectLoadedEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The loaded project. Now also available from the <see cref="IStudioEnvironment.CurrentProject"/> property.
        /// </summary>
        public Project LoadedProject { get; }

        public ProjectLoadedEventArgs(IStudioEnvironment environment, Project project) : base(environment)
        {
            LoadedProject = project;
        }
    }
}
