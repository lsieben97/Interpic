using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Interpic.Settings
{
    public class SettingsCollection : INotifyPropertyChanged
    {
        private List<Setting<int>> _numeralSettings = new List<Setting<int>>();
        private List<Setting<string>> _textSettings = new List<Setting<string>>();
        private List<Setting<bool>> _booleanSettings = new List<Setting<bool>>();
        private List<MultipleChoiceSetting> _multipleChoiceSettings = new List<MultipleChoiceSetting>();
        private List<PathSetting> _pathSettings = new List<PathSetting>();
        private List<Setting<SettingsCollection>> _subSettings = new List<Setting<SettingsCollection>>();

        public string Name { get; set; }
        public string Id { get; set; }

        public List<Setting<int>> NumeralSettings { get => _numeralSettings; set { _numeralSettings = value; RaisePropertyChanged("NumeralSettings"); } }
        public List<Setting<string>> TextSettings { get => _textSettings; set { _textSettings = value; RaisePropertyChanged("TextSettings"); } }
        public List<Setting<bool>> BooleanSettings { get => _booleanSettings; set { _booleanSettings = value; RaisePropertyChanged("BooleanSettings"); } }
        public List<MultipleChoiceSetting> MultipleChoiceSettings { get => _multipleChoiceSettings; set { _multipleChoiceSettings = value; RaisePropertyChanged("MultipleChoiseSettings"); } }
        public List<PathSetting> PathSettings { get => _pathSettings; set { _pathSettings = value; RaisePropertyChanged("PathSettings"); } }
        public List<Setting<SettingsCollection>> SubSettings { get => _subSettings; set { _subSettings = value; RaisePropertyChanged("SubSettings"); } }

        #region Getters
        public int GetNumeralSetting(string key)
        {
            foreach(Setting<int> setting in NumeralSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return int.MinValue;
        }

        public string GetTextSetting(string key)
        {
            foreach (Setting<string> setting in TextSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return null;
        }

        public bool GetBooleanSetting(string key)
        {
            foreach (Setting<bool> setting in BooleanSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return false;
        }

        public string GetMultipleChoiceSetting(string key)
        {
            foreach (Setting<string> setting in MultipleChoiceSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return null;
        }

        public string GetPathSetting(string key)
        {
            foreach (Setting<string> setting in PathSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return null;
        }

        public SettingsCollection GetSubSettings(string key)
        {
            foreach (Setting<SettingsCollection> setting in SubSettings)
            {
                if (setting.Key == key)
                {
                    return setting.Value;
                }
            }
            return null;
        }

        public bool Validate()
        {
            bool valid = true;

            foreach (Setting<string> setting in TextSettings)
            {
                if (setting.Value == null)
                {
                    valid = false;
                }
            }

            foreach (MultipleChoiceSetting setting in MultipleChoiceSettings)
            {
                if (setting.Value == null)
                {
                    valid = false;
                }
            }

            foreach (PathSetting setting in PathSettings)
            {
                if (setting.Value == null)
                {
                    valid = false;
                }
            }

            foreach (Setting<SettingsCollection> setting in SubSettings)
            {
                if (setting.Value.Validate() == false)
                {
                    valid = false;
                }
            }

            return valid;
        }
        #endregion

        public static SettingsChanges GetChanges(SettingsCollection oldCollection, SettingsCollection newCollection)
        {
            SettingsChanges changes = new SettingsChanges();

            foreach (Setting<string> setting in oldCollection.TextSettings)
            {
                if (setting.Value != newCollection.GetTextSetting(setting.Key))
                {
                    changes.TextChanges.Add(setting.Key);
                    changes.Any = true;
                }
            }

            foreach (Setting<int> setting in oldCollection.NumeralSettings)
            {
                if (setting.Value != newCollection.GetNumeralSetting(setting.Key))
                {
                    changes.NumeralChanges.Add(setting.Key);
                    changes.Any = true;
                }
            }

            foreach (Setting<bool> setting in oldCollection.BooleanSettings)
            {
                if (setting.Value != newCollection.GetBooleanSetting(setting.Key))
                {
                    changes.BooleanChanges.Add(setting.Key);
                    changes.Any = true;
                }
            }

            foreach (MultipleChoiceSetting setting in oldCollection.MultipleChoiceSettings)
            {
                if (setting.Value != newCollection.GetMultipleChoiceSetting(setting.Key))
                {
                    changes.MultipleChoiceChanges.Add(setting.Key);
                    changes.Any = true;
                }
            }

            foreach (PathSetting setting in oldCollection.PathSettings)
            {
                if (setting.Value != newCollection.GetPathSetting(setting.Key))
                {
                    changes.PathChanges.Add(setting.Key);
                    changes.Any = true;
                }
            }

            foreach (Setting<SettingsCollection> setting in oldCollection.SubSettings)
            {
                SettingsChanges subChanges = GetChanges(setting.Value, newCollection.GetSubSettings(setting.Key));
                if (subChanges.Any)
                {
                    changes.SubSettingsChanges.Add(setting.Key, subChanges);
                }
            }

            if (changes.SubSettingsChanges.Any(subChanges => subChanges.Value.Any == true))
            {
                changes.Any = true;
            }

            return changes;
        }

        #region *** INotifyPropertyChanged Members and Invoker ***
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
