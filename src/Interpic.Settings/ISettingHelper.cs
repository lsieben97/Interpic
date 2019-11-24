namespace Interpic.Settings
{
    public interface ISettingHelper<T>
    {
        string HelpButtonText { get; set; }
        HelpResult<T> Help(T lastValue);
    }
}
