using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web
{
    class WebExtension : Extension
    {
        static internal WebExtension Instance = new WebExtension();
        public override string GetDescription()
        {
            return "Website project type extension. Provides access to websites.";
        }

        public override ExtensionType GetExtensionType()
        {
            return ExtensionType.ProjectType;
        }

        public override string GetName()
        {
            return "Interpic Web";
        }
    }
}
