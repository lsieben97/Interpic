using Interpic.Models;
using Interpic.Utils;
using System.Windows.Media;

namespace Interpic.Studio.StudioViews
{
    public class ControlStudioView : StudioView<ControlView>
    {
        public override string Title { get; set; } = "Control";
        public override ImageSource Icon { get; set; } = ImageUtils.ImageFromString("ControlWhite.png");
        public override ControlView View { get; set; }

        public ControlStudioView(Control control)
        {
            View = new ControlView(control);
        }
    }
}
