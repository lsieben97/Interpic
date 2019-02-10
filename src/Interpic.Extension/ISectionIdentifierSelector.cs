using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Extensions
{
    public interface ISectionIdentifierSelector
    {
        SectionIdentifier SectionIdentifier { get; set; }
        Page Page { get; set; }

        void ShowSelector();
    }
}
