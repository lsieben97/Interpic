namespace Interpic.Models.EventArgs
{
    public class SectionEventArgs : InterpicStudioEventArgs
    {
        public Section Section { get; }

        public SectionEventArgs(IStudioEnvironment environment, Section section) : base(environment)
        {
            Section = section;
        }
    }
}
