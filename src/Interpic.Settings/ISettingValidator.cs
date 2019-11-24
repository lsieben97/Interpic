namespace Interpic.Settings
{
    public interface ISettingValidator<T>
    {
        SettingValidationResult Validate(T value);
    }
}
