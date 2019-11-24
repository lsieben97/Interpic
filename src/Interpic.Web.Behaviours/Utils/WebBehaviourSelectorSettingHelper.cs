using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Behaviours.Windows;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Interpic.Web.Behaviours.Utils
{
    public class WebBehaviourSelectorSettingHelper : ISettingHelper<string>
    {
        public string HelpButtonText { get; set; } = "Select";

        public HelpResult<string> Help(string lastValue)
        {
            HelpResult<string> result = new HelpResult<string>();

            PickWebBehaviours dialog = null;

            if (string.IsNullOrWhiteSpace(lastValue))
            {
                dialog = new PickWebBehaviours(WebBehaviour.AvailableBehaviours);
            }
            else
            {
                dialog = new PickWebBehaviours(WebBehaviour.AvailableBehaviours, JsonConvert.DeserializeObject<List<WebBehaviour>>(lastValue));
            }

            dialog.ShowDialog();

            if (dialog.SelectedBehaviours != null)
            {
                result.Result = JsonConvert.SerializeObject(dialog.SelectedBehaviours.ToList(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            }
            else
            {
                result.Canceled = true;
            }

            return result;
        }
    }
}
