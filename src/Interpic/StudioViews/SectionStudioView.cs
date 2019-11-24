using Interpic.Models;
using Interpic.Utils;
using System.Windows.Media;

namespace Interpic.Studio.StudioViews
{
    public class SectionStudioView : StudioView<SectionView>
    {
        public override string Title { get; set; } = "Section";
        public override ImageSource Icon { get; set; } = ImageUtils.ImageFromString("SectionWhite.png");
        public override SectionView View { get; set; }

        public SectionStudioView(Section section)
        {
            View = new SectionView(section);
        }
    }
}
