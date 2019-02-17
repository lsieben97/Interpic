using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class Version : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private SettingsCollection _settings;
        private string _locale;
        private ObservableCollection<Page> _pages;

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }
        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }
        public bool HasSettingsAvailable { get => Settings != null; }
        public string Locale { get => _locale; set { _locale = value; RaisePropertyChanged("Locale"); } }
        public ObservableCollection<Page> Pages { get => _pages; set { _pages = value; RaisePropertyChanged("Pages"); } }
        public bool IsCurrent { get; set; }
        [IgnoreChangeListener]
        [JsonIgnore]
        public Project Parent { get; set; }
        #region *** INotifyPropertyChanged Members and Invoker ***
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
