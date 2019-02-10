namespace Interpic.Models
{
    public interface ILogger
    {
        void LogInfo(string message, string source = "Interpic Studio extension");
        void LogWarning(string message, string source = "Interpic Studio extension");
        void LogError(string message, string source = "Interpic Studio extension");
        void LogDebug(string message, string source = "Interpic Studio extension");
        string GetLog();
    }
}