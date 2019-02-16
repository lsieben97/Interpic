using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Xml;

namespace Interpic.Models
{
    public class Page : INotifyPropertyChanged
    {
        private string _name;
        private string _type;
        private string _text;
        private ObservableCollection<Section> _sections = new ObservableCollection<Section>();
        private SettingsCollection _settings;
        private string _source;
        private ObservableCollection<Control> _controls = new ObservableCollection<Control>();
        private byte[] _screenshot;
        private ExtensionObject _extensions;

        /// <summary>
        /// The name of the page.
        /// </summary>
        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }

        /// <summary>
        /// The type of the page.
        /// </summary>
        public string Type { get => _type; set { _type = value; RaisePropertyChanged("Type"); } }

        /// <summary>
        /// The text of the page if the page is a text page.
        /// </summary>
        public string Description { get => _text; set { _text = value; RaisePropertyChanged("Text"); } }

        /// <summary>
        /// The source for this page.
        /// </summary>
        public string Source { get => _source; set { _source = value; RaisePropertyChanged("Source"); } }

        /// <summary>
        /// The sections of the page.
        /// </summary>
        public ObservableCollection<Section> Sections { get => _sections; set { _sections = value; RaisePropertyChanged("Sections"); } }

        /// <summary>
        /// Whether the page has settings available.
        /// When false, <see cref="Settings"/> = <code>null</code>.
        /// </summary>
        public bool HasSettingsAvailable { get => Settings != null; }

        /// <summary>
        /// The screenshot of this page.
        /// </summary>
        public byte[] Screenshot { get => _screenshot; set { _screenshot = value; RaisePropertyChanged("Screenshot"); } }

        [IgnoreChangeListener]
        [JsonIgnore]
        public Project Parent { get; set; }

        /// <summary>
        /// The settings of the page.
        /// </summary>
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Occurs before the page settings window is shown for this page.
        /// Changes made to the settings will be visible in the window.
        /// This event fires after the <see cref="IStudioEnvironment.PageSettingsOpening"/> event.
        /// </summary>
        public event OnPageSettingsOpening SettingsOpening;

        [JsonIgnore]
        public TreeViewItem TreeViewItem { get; set; }

        /// <summary>
        /// Occurs after the page settings window is shown for this page.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event fires before the <see cref="IStudioEnvironment.PageSettingsOpened"/> event;
        /// </summary>
        public event OnPageSettingsOpened SettingsOpened;

        [JsonIgnore]
        public ExtensionObject Extensions
        {
            get => _extensions;
            set
            {
                bool raiseChangedEvent = false;
                _extensions = value;
                if (_extensions != null)
                {
                    raiseChangedEvent = _extensions.RaisePropertyChanged;
                }
                else
                {
                    if (value != null)
                    {
                        raiseChangedEvent = value.RaisePropertyChanged;
                    }

                }
                if (raiseChangedEvent)
                {
                    RaisePropertyChanged("Extensions");
                }
            }
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