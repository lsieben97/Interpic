using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Settings
{
    public interface ISettingHelper<T>
    {
        string HelpButtonText { get; set; }
        HelpResult<T> Help(T lastValue);
    }
}
