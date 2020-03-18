namespace Interpic.Models.Behaviours
{
    public abstract class CheckAction : Action
    {
        protected CheckAction()
        {
            Type = ActionType.Check;
        }

        public Behaviour BehaviourWhenTrue { get; set; }
        public Behaviour BehaviourWhenFalse { get; set; }

        public bool CheckResult { get; set; }
    }
}
