﻿using Interpic.Models.Behaviours;
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
    public class Control : INotifyPropertyChanged
    {
        private string _name;
        private SettingsCollection _settings;
        private ControlIdentifier _sourceNode;
        private ElementBounds _elementBounds;
        private ExtensionObject _extensions;
        private string _description;
        private bool isLoaded;
        private ObservableCollection<Behaviour> behaviours = new ObservableCollection<Behaviour>();

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

        public ObservableCollection<Behaviour> Behaviours { get => behaviours; set { behaviours = value; RaisePropertyChanged("Behaviours"); } }

        public List<string> BehaviourIds { get; set; }

        /// <summary>
        /// Whether the control has settings.
        /// When false, <see cref="Settings"/> = <code>null</code>.
        /// </summary>
        public bool HasSettingsAvailable { get => Settings != null; }

        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool IsLoaded { get => isLoaded; set { isLoaded = value; RaisePropertyChanged("IsLoaded"); } }

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

        /// <summary>
        /// Occurs when the control is about to be deleted. This is the last time the <see cref="Models.Control"/> object is available.
        /// This event occurs after the <see cref="IStudioEnvironment.ControlRemoved"/> event.
        /// </summary>
        public event OnControlRemoved Removed;

        public void FireSettingsOpeningEvent(object sender, ControlSettingsEventArgs e)
        {
            SettingsOpening?.Invoke(sender, e);
        }

        public void FireSettingsOpenedEvent(object sender, ControlSettingsEventArgs e)
        {
            SettingsOpened?.Invoke(sender, e);
        }

        public void FireRemovedEvent(object sender, ControlEventArgs e)
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