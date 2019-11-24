using Interpic.Models;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interpic.Studio.StudioViews
{
    public class ProjectStudioView : StudioView<ProjectView>
    {
        public override string Title { get; set; } = "Project";
        public override ImageSource Icon { get; set; } = new BitmapImage(new Uri("/Interpic.UI;component/Icons/ProjectWhite.png", UriKind.RelativeOrAbsolute));
        public override ProjectView View { get; set; }

        public ProjectStudioView(Models.Version project)
        {
            View = new ProjectView(project);
        }

    }
}
