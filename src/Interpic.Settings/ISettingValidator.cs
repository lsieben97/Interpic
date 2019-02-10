using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Settings
{
    public interface ISettingValidator<T>
    {
        SettingValidationResult Validate(T value);
    }
}
