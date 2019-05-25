using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Extensions
{
    public interface IExtensionRegistry
    {
        void RegisterProjectType(IProjectTypeProvider provider);
        void RegisterProjectBuilder(IProjectBuilder builder);
    }
}
