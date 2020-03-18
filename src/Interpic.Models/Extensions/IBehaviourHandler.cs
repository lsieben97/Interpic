using Interpic.Models.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Extensions
{
    public interface IBehaviourHandler<BehaviourType, ActionType> where BehaviourType : Behaviour
                                                                  where ActionType : Behaviours.Action
    {
        bool SaveBehaviour(BehaviourType behaviour);
        bool SaveToFile(List<BehaviourType> behaviours, string path);
        List<BehaviourType> LoadBehaviours();
        List<BehaviourType> LoadFromFile(string path);
        bool ExecuteBehaviours(List<BehaviourType> behaviours);
        List<BehaviourType> GetAvailableBehaviours();
        List<ActionType> GetAvailableActions();
    }
}
