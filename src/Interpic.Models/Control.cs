using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Xml;

namespace Interpic.Models
{
    public class Control : INotifyPropertyChanged
    {
        private string _name;
        private SettingsCollection _settings;
        private ControlIdentifier _sourceNode;
        private ElementBounds _elementBounds;
        private ExtensionObject _extensions;
        private string _description;

        /// <summary>
        /// The name of the control.
        /// </summary>
        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }

        /// <summary>
        /// The node from the manual source this section represents. 
        /// </summary>
        public ControlIdentifier Identifier { get => _sourceNode; set { _sourceNode = value; RaisePropertyChanged("SourceNode"); } }

        /// <summary>
        /// The bounds of the element.
        /// </summary>
        public ElementBounds ElementBounds { get => _elementBounds; set { _elementBounds = value; RaisePropertyChanged("ElementBounds"); } }

        [JsonIgnore]
        public TreeViewItem TreeViewItem { get; set; }

        [IgnoreChangeListener]
        [JsonIgnore]
        public Section Parent { get; set; }

        /// <summary>
        /// The settings of the control.
        /// </summary>
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }

        /// <summary>
        /// Whether the control has settings.
        /// When false, <see cref="Settings"/> = <code>null</code>.
        /// </summary>
        public bool HasSettingsAvailable { get => Settings != null; }

        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }

        /// <summary>
        /// Occurs before the control settings window is shown for this control.
        /// Changes made to the settings will be visible in the window.
        /// This event fires after the <see cref="IStudioEnvironment.ControlSettingsOpening"/> event.
        /// </summary>
        public event OnControlSettingsOpening SettingsOpening;

        /// <summary>
        /// Occurs after the control settings window is shown for this control.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event fires before the <see cref="IStudioEnvironment.ControlSettingsOpened"/> event;
        /// </summary>
        public event OnControlSettingsOpened SettingsOpened;

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