using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interpic.StudioViews
{
    public class WelcomeStudioView : StudioView<WelcomeView>
    {
        public override string Title { get; set; } = "Welcome";
        public override ImageSource Icon { get; set; } = new BitmapImage(new Uri("/Interpic.UI;component/Icons/InfoWhite.png", UriKind.RelativeOrAbsolute));
        public override WelcomeView View { get; set; } = new WelcomeView();
    }
}
