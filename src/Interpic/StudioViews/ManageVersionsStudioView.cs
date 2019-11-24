using Interpic.Models;
using Interpic.Models.Extensions;
using Interpic.Studio.Windows;
using Interpic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Interpic.Studio.StudioViews
{
    class ManageVersionsStudioView : StudioView<ManageVersionsView>
    {
        public override string Title { get; set; } = "Manage Versions";
        public override ImageSource Icon { get; set; } = ImageUtils.ImageFromString("PageWhite.png");
        public override ManageVersionsView View { get; set; }

        public ManageVersionsStudioView(Project project, IProjectTypeProvider provider, IStudioEnvironment studio)
        {
            View = new ManageVersionsView(project, provider, studio);
        }
    }
}
