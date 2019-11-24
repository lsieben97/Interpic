using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Behaviours.Windows;
using Newtonsoft.Json;

namespace Interpic.Web.Behaviours.Utils
{
    public class ElementSelectorSettingHelper : ISettingHelper<string>
    {
        public string HelpButtonText { get; set; } = "Select element";

        public HelpResult<string> Help(string lastValue)
        {
            HelpResult<string> result = new HelpResult<string>();
            ElementSelectorEditor editor;
            if (!string.IsNullOrWhiteSpace(lastValue))
            {
                editor = new ElementSelectorEditor(JsonConvert.DeserializeObject<ElementSelector>(lastValue));
            }
            else
            {
                editor = new ElementSelectorEditor();
            }

            editor.ShowDialog();
            if (editor.Selector != null)
            {
                result.Result = JsonConvert.SerializeObject(editor.Selector);
                return result;
            }
            else
            {
                result.Canceled = true;
                return result;
            }
        }
    }
}
