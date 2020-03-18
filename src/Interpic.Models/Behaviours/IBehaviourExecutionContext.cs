using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Behaviours
{
    public interface IBehaviourExecutionContext
    {
        T GetExecutionHelper<T>(string name) where T : class;

        bool HasExecutionHelper(string name);
        IStudioEnvironment GetStudioEnvironment();
        ProjectState GetProjectState { get; set; }
    }
}
