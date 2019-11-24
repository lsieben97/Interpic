using Interpic.Models;
using Interpic.Utils;
using System.Windows.Media;

namespace Interpic.Studio.StudioViews
{
    public class PageStudioView : StudioView<PageView>
    {
        public override string Title { get; set; } = "Page";
        public override ImageSource Icon { get; set; } = ImageUtils.ImageFromString("PageWhite.png");
        public override PageView View { get; set; }

        public PageStudioView(Page page)
        {
            View = new PageView(page);
        }
    }
}
