namespace Interpic.Models.Behaviours
{
    internal class ExecuteWebActionTask
    {
        private Action action;
        private IBehaviourExecutionContext context;

        public ExecuteWebActionTask(Action action, IBehaviourExecutionContext context)
        {
            this.action = action;
            this.context = context;
        }
    }
}