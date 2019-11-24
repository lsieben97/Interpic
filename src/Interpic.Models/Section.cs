using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace Interpic.Models
{
    public class Section : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Control> _controls = new ObservableCollection<Control>();
        private SettingsCollection _settings;
        private SectionIdentifier _sourceNode;
        private string _description;
        private ObservableCollection<DiscoveredControl> _discoveredControls = new ObservableCollection<DiscoveredControl>();
        private ElementBounds _elementBounds;
        private ExtensionObject _extensions;
        private bool isLoaded;

        /// <summary>
        /// The name of the Section.
        /// </summary>
        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }

        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }

        /// <summary>
        /// The controls of the section.
        /// </summary>
        public ObservableCollection<Control> Controls { get => _controls; set { _controls = value; RaisePropertyChanged("Controls"); } }

        /// <summary>
        /// The settings of the section.
        /// </summary>
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }

        /// <summary>
        /// Whether the section has settings.
        /// When false, <see cref="Settings"/> = <code>null</code>.
        /// </summary>
        public bool HasSettingsAvailable { get => Settings != null; }

        /// <summary>
        /// The node from the manual source this section represents. 
        /// </summary>
        public SectionIdentifier SectionIdentifier { get => _sourceNode; set { _sourceNode = value; RaisePropertyChanged("SourceNode"); } }

        /// <summary>
        /// The bounds of the element.
        /// </summary>
        public ElementBounds ElementBounds { get => _elementBounds; set { _elementBounds = value; RaisePropertyChanged("ElementBounds"); } }

        [JsonIgnore]
        public TreeViewItem TreeViewItem { get; set; }

        [IgnoreChangeListener]
        [JsonIgnore]
        public Page Parent { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool IsLoaded { get => isLoaded; set { isLoaded = value; RaisePropertyChanged("IsLoaded"); } }

        /// <summary>
        /// The list of automatically dicovered controls for this page.
        /// </summary>
        public ObservableCollection<DiscoveredControl> DiscoveredControls { get => _discoveredControls; set { _discoveredControls = value; RaisePropertyChanged("DiscoveredControls"); } }

        /// <summary>
        /// Occurs before the section settings window is shown for this section.
        /// Changes made to the settings will be visible in the window.
        /// This event fires after the <see cref="IStudioEnvironment.SectionSettingsOpening"/> event.
        /// </summary>
        public event OnSectionSettingsOpening SettingsOpening;

        /// <summary>
        /// Occurs after the section settings window is shown for this section.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event fires before the <see cref="IStudioEnvironment.SectionSettingsOpened"/> event;
        /// </summary>
        public event OnSectionSettingsOpened SettingsOpened;

        /// <summary>
        /// Occurs when the section is about to be deleted. This is the last time the <see cref="Models.Section"/> object is available.
        /// This event occurs after the <see cref="IStudioEnvironment.SectionRemoved"/> event.
        /// </summary>
        public event OnSectionRemoved Removed;

        public void FireSettingsOpeningEvent(object sender, SectionSettingsEventArgs e)
        {
            SettingsOpening?.Invoke(sender, e);
        }

        public void FireSettingsOpenedEvent(object sender, SectionSettingsEventArgs e)
        {
            SettingsOpened?.Invoke(sender, e);
        }

        public void FireRemovedEvent(object sender, SectionEventArgs e)
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