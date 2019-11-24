using System.Windows.Controls;
using System.Windows.Media;

namespace Interpic.Models
{
    public abstract class StudioView<T> where T : UserControl, IStudioViewHandler
    {
        public abstract string Title { get; set; }
        public abstract ImageSource Icon { get; set; }
        public abstract T View { get; set; }
        
    }
}
