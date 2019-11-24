namespace Interpic.Models.EventArgs
{
    public class VersionEventArgs : InterpicStudioEventArgs
    {
        public VersionEventArgs(IStudioEnvironment environment, Version version) : base(environment)
        {
            Version = version;
        }

        public Models.Version Version { get; set; }
    }
}
