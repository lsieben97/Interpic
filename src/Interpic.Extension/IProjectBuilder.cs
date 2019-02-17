using Interpic.Models;
using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Extensions
{
    public interface IProjectBuilder
    {
        /// <summary>
        /// Get the unique identifier for this project builder.
        /// </summary>
        /// <returns>The unique identifier for this project builder.</returns>
        string GetBuilderId();

        /// <summary>
        /// Get the name of this project builder.
        /// </summary>
        /// <returns>The name of the project builder.</returns>
        string GetBuilderName();

        /// <summary>
        /// Get the description of this project builder.
        /// </summary>
        /// <returns>The description of this project builder.</returns>
        string GetBuilderDescription();

        /// <summary>
        /// The studio environment.
        /// </summary>
        IStudioEnvironment Studio { get; set; }


        /// <summary>
        /// Build the given project.
        /// </summary>
        /// <param name="options">Options for building the project.</param>
        /// <param name="project">The project to build.</param>
        /// <returns></returns>
        bool Build(BuildOptions options, Project project, Models.Version version);

        bool CleanOutputDirectory(Project project);

        /// <summary>
        /// Get a list of compatible project types. <code>null</code> indicates all project types are accepted.
        /// </summary>
        /// <returns>A list of compatible project types</returns>
        List<string> GetCompatibleProjectTypes();

        SettingsCollection GetBuildSettings(Project project);
    }
}
