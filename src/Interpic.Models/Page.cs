using Interpic.Models.Behaviours;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace Interpic.Models
{
    public class Page : INotifyPropertyChanged
    {
        public const string PAGE_TYPE_TEXT = "text";
        public const string PAGE_TYPE_REFERENCE = "reference";

        private string _name;
        private string _type;
        private string _text;
        private ObservableCollection<Section> _sections = new ObservableCollection<Section>();
        private SettingsCollection _settings;
        private string _source;
        private ObservableCollection<Control> _controls = new ObservableCollection<Control>();
        private byte[] _screenshot;
        private ExtensionObject _extensions;
        private List<string> _siblingPageIds;
        private bool isLoaded;
        private ObservableCollection<Behaviour> behaviours = new ObservableCollection<Behaviour>();

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
        public Version Parent { get; set; }

        /// <summary>
        /// Get the id's from the pages that are siblings (the same page in other versions)
        /// </summary>
        public List<string> SiblingPageIds { get => _siblingPageIds; set { _siblingPageIds = value; RaisePropertyChanged("SiblingPageIds"); } }

        /// <summary>
        /// The settings of the page.
        /// </summary>
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public ObservableCollection<Behaviour> Behaviours { get => behaviours; set { behaviours = value; RaisePropertyChanged("Behaviours"); } }

        public List<string> BehaviourIds { get; set; }

        /// <summary>
        /// Occurs before the page settings window is shown for this page.
        /// Changes made to the settings will be visible in the window.
        /// This event fires after the <see cref="IStudioEnvironment.PageSettingsOpening"/> event.
        /// </summary>
        public event OnPageSettingsOpening SettingsOpening;

        /// <summary>
        /// Occurs when the page is about to be deleted. This is the last time the <see cref="Models.Page"/> object is available.
        /// This event occurs after the <see cref="IStudioEnvironment.PageRemoved"/> event.
        /// </summary>
        public event OnPageRemoved Removed;

        public bool IsLoaded { get => isLoaded; set { isLoaded = value; RaisePropertyChanged("IsLoaded"); } }

        [JsonIgnore]
        public TreeViewItem TreeViewItem { get; set; }

        /// <summary>
        /// Occurs after the page settings window is shown for this page.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event fires before the <see cref="IStudioEnvironment.PageSettingsOpened"/> event;
        /// </summary>
        public event OnPageSettingsOpened SettingsOpened;

        public void FireSettingsOpeningEvent(object sender, PageSettingsEventArgs e)
        {
            SettingsOpening?.Invoke(sender, e);
        }

        public void FireSettingsOpenedEvent(object sender, PageSettingsEventArgs e)
        {
            SettingsOpened?.Invoke(sender, e);
        }

        public void FireRemovedEvent(object sender, PageEventArgs e)
        {
            Removed?.Invoke(sender, e);
        }

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