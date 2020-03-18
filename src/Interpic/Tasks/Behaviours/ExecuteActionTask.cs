using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Models.Behaviours;
using Interpic.Web.Behaviours.Models;
using System;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks.Behaviours
{
    public class ExecuteActionTask : AsyncTask
    {
        private readonly IStudioEnvironment studio;

        public Models.Behaviours.Action WebAction { get; set; }
        public IBehaviourExecutionContext Context { get; set; }
        public ExecuteActionTask(Models.Behaviours.Action webAction, IBehaviourExecutionContext context, IStudioEnvironment studio)
        {
            WebAction = webAction;
            Context = context;
            this.studio = studio;
            TaskName = "Executing web action...";
            TaskDescription = WebAction.Name;
            ActionName = WebAction.Name;
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                WebAction.Execute(Context);
                if (WebAction.Type == Models.Behaviours.Action.ActionType.Check)
                {
                    CheckAction checkWebAction = (CheckAction)WebAction;
                    Behaviour behaviourToExecute = checkWebAction.CheckResult ? checkWebAction.BehaviourWhenTrue : checkWebAction.BehaviourWhenFalse;
                    if (behaviourToExecute != null)
                    {
                        studio.ExecuteBehaviour(behaviourToExecute,Context);
                    }
                }
            }
            catch(Exception ex)
            {
                Dialog.CancelAllTasks($"Error while executing action: {ex.Message}");
            }
        }
    }
}
