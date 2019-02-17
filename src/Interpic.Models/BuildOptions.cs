﻿using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class BuildOptions
    {
        /// <summary>
        /// The type of build.
        /// </summary>
        public BuildType Type { get; set; }

        public List<Version> VersionsToBuild { get; set; }

        /// <summary>
        /// Whether to clean the output directory existing files.
        /// </summary>
        public bool CleanOutputDirectory { get; set; }

        /// <summary>
        /// Aditional build settings.
        /// </summary>
        public SettingsCollection BuildSettings { get; set; }

        public enum BuildType
        {
            EntireManualSpecificVersions,
            EntireManualAllVersions,
        }
    }
}
