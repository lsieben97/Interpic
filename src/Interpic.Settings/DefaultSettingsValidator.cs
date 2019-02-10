using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Settings
{
    public class DefaultSettingsValidator<T> : ISettingValidator<T>
    {
        public SettingValidationResult Validate(T value)
        {
            return new SettingValidationResult
            {
                IsValid = true
            };
        }
    }
}
