using System.Collections.Generic;
using System.ComponentModel;

namespace Interpic.Settings
{
    public class MultipleChoiceSetting : Setting<string>, INotifyPropertyChanged
    {
        private List<KeyValuePair<string, string>> _choices;

        public List<KeyValuePair<string, string>> Choices { get => _choices; set { _choices = value; RaisePropertyChanged("Choices"); } }

    }
}