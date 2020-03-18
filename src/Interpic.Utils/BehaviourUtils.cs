using Interpic.Models.Behaviours;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Utils
{
    public static class BehaviourUtils
    {
        public static List<string> GetBehaviourIds(ObservableCollection<Behaviour> behaviours)
        {
            List<string> result = new List<string>();
            foreach (Behaviour behaviour in behaviours)
            {
                result.Add(behaviour.Id);
            }
            return result;
        }
    }
}
