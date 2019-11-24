using Interpic.Models;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interpic.Studio.StudioViews
{
    public class HomeStudioView : StudioView<HomeView>
    {
        public override string Title { get; set; } = "Home";
        public override ImageSource Icon { get; set; } = new BitmapImage(new Uri("/Interpic.UI;component/Icons/HomeWhite.png", UriKind.RelativeOrAbsolute));
        public override HomeView View { get; set; } = new HomeView();
    }
}
