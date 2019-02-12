using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Settings
{
    public class HelpResult<T>
    {
        public T Result { get; set; }
        public bool Canceled { get; set; }
    }
}
