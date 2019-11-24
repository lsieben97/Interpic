using System.Collections.Generic;

namespace Interpic.Settings
{
    public class SettingsChanges
    {
        public bool Any { get; set; }
        public List<string> BooleanChanges { get; set; } = new List<string>();
        public List<string> NumeralChanges { get; set; } = new List<string>();
        public List<string> MultipleChoiceChanges { get; set; } = new List<string>();
        public List<string> TextChanges { get; set; } = new List<string>();
        public List<string> PathChanges { get; set; } = new List<string>();
        public Dictionary<string, SettingsChanges> SubSettingsChanges { get; set; } = new Dictionary<string, SettingsChanges>();
    }
}
