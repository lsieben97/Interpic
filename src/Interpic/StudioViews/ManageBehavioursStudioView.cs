using Interpic.Models;
using Interpic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Interpic.Studio.StudioViews
{
    public class ManageBehavioursStudioView : StudioView<ManageBehavioursView>
    {
        public override string Title { get; set; } = "Manage Behaviours";
        public override ImageSource Icon { get; set; } = ImageUtils.ImageFromString("BehaviourWhite.png");
        public override ManageBehavioursView View { get; set; }

        public ManageBehavioursStudioView(Project project)
        {
            View = new ManageBehavioursView(project);
        }
    }
}
