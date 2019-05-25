using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Extensions
{
    public interface IControlIdentifierSelector
    {
        ControlIdentifier ControlIdentifier { get; set; }

        Section Section { get; set; }

        void ShowSelector();
    }
}
