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
