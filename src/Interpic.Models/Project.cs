﻿using Interpic.Models.Behaviours;
using Interpic.Settings;
using Interpic.Studio.RecursiveChangeListener;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace Interpic.Models
{
    public class Project : INotifyPropertyChanged
    {
        private string _name;
        private string _path;
        private string _type;
        private string _outputType;
        private SettingsCollection _settings;
        private ObservableCollection<ExtensionDeclaration> _projectExtensions;
        private string _typeProviderId;
        private DateTime _lastSaved;

        private string _outputFolder;
        private string _projectFolder;
        private ExtensionObject _extensions;
        private ObservableCollection<Version> _versions;
        private ObservableCollection<TrustedAssembly> _trustedAssemblies = new ObservableCollection<TrustedAssembly>();
        private BehaviourConfiguration behaviourConfiguration;

        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }
        public string Path { get => _path; set { _path = value; RaisePropertyChanged("Path"); } }
        public string Type { get => _type; set { _type = value; RaisePropertyChanged("Type"); } }
        public string OutputType { get => _outputType; set { _outputType = value; RaisePropertyChanged("OutputType"); } }
        public SettingsCollection Settings { get => _settings; set { _settings = value; RaisePropertyChanged("Settings"); } }
        public ObservableCollection<ExtensionDeclaration> ProjectExtensions { get => _projectExtensions; set { _projectExtensions = value; RaisePropertyChanged("ProjectSettings"); } }
        public string TypeProviderId { get => _typeProviderId; set { _typeProviderId = value; RaisePropertyChanged("TypeProviderId"); } }
        public DateTime LastSaved { get => _lastSaved; set { _lastSaved = value; RaisePropertyChanged("LastSaved"); } }
        public ObservableCollection<Version> Versions { get => _versions; set { _versions = value; RaisePropertyChanged("Versions"); } }
        public string OutputFolder { get => _outputFolder; set { _outputFolder = value; RaisePropertyChanged("OutputFolder"); } }
        public string ProjectFolder { get => _projectFolder; set { _projectFolder = value; RaisePropertyChanged("ProjectFolder"); } }
        public ObservableCollection<TrustedAssembly> TrustedAssemblies { get => _trustedAssemblies; set { _trustedAssemblies = value; RaisePropertyChanged("TrustedAssemblies"); } }

        [JsonIgnore]
        public bool Changed { get; set; }

        [IgnoreChangeListener]
        public string LastViewedVersionId { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public bool IsNew { get; set; }

        [JsonIgnore]
        public TreeViewItem TreeViewItem { get; set; }

        public bool HasSettingsAvailable { get => Settings != null; }

        [JsonIgnore]
        public BehaviourConfiguration BehaviourConfiguration { get => behaviourConfiguration; set { behaviourConfiguration = value; RaisePropertyChanged("BehaviourConfiguration"); } }

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