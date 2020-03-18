using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio
{
    internal class ExtensionRepsentingStudioEnvironment : Extension
    {
        public override string GetDescription()
        {
            return "Studio";
        }

        public override ExtensionType GetExtensionType()
        {
            return ExtensionType.Builder;
        }

        public override string GetName()
        {
            return "Studio";
        }
    }
}
