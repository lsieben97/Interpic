namespace Interpic.Models.EventArgs
{
    public class ControlEventArgs : InterpicStudioEventArgs
    {
        public Control Control { get; }

        public ControlEventArgs(IStudioEnvironment environment, Control control) : base(environment)
        {
            Control = control;
        }
    }
}
