namespace Interpic.UI.Controls
{
    public interface IStudioTab
    {
        bool DoesContainChanges { get; }
        bool ForceClose { get; set; }
        void ContainsChanges(bool changes);
        void SetTitle(string title);
        void CloseTab();
        void Focus();
    }
}
