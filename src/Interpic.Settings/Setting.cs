using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Settings
{
    public class Setting<T> : INotifyPropertyChanged
    {
        private string _key;
        private string _name;
        private string _description;
        private bool _hidden;
        private T _value;

        public string Key { get => _key; set { _key = value; RaisePropertyChanged("Key"); } }
        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }
        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }
        public bool Hidden { get => _hidden; set { _hidden = value; RaisePropertyChanged("Hidden"); } }
        public T Value { get => _value; set { _value = value; RaisePropertyChanged("Value"); } }
        [JsonIgnore]
        public ISettingValidator<T> Validator { get; set; } = new DefaultSettingsValidator<T>();
        [JsonIgnore]
        public ISettingHelper<T> Helper { get; set; }
        [JsonIgnore]
        internal TextBlock InvalidLabel { get; set; }
        [JsonIgnore]
        internal UIElement Control { get; set; }

        public new Type GetType()
        {
            return typeof(T);
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