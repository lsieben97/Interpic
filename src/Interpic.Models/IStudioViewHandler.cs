using Interpic.UI.Controls;

namespace Interpic.Models
{
    public interface IStudioViewHandler
    {
        IStudioTab StudioTab { get; set; }
        IStudioEnvironment Studio { get; set; }
        void ViewAttached();
        void ViewDetached();
        string GetTabContents();
    }
}
