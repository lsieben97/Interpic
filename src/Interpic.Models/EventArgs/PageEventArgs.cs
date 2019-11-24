namespace Interpic.Models.EventArgs
{
    public class PageEventArgs : InterpicStudioEventArgs
    {
        public Page Page { get; }

        public PageEventArgs(IStudioEnvironment environment, Page page) : base(environment)
        {
            Page = page;
        }
    }
}
