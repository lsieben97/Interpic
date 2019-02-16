using Interpic.Models;
using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Extensions
{
    public interface IProjectTypeProvider
    {
        /// <summary>
        /// Get the unique identifier for this project type.
        /// </summary>
        /// <returns>The unique identifier for this project type.</returns>
        string GetProjectTypeId();

        /// <summary>
        /// Get the name of this project type.
        /// </summary>
        /// <returns>The name of the project type.</returns>
        string GetProjectTypeName();

        /// <summary>
        /// Get the description of this project type.
        /// </summary>
        /// <returns>The description of this project type.</returns>
        string GetProjectTypeDescription();

        /// <summary>
        /// Get a source provider capable of loading manual sources for this project type.
        /// </summary>
        /// <returns>A source provider capable of loading manual soures for this project type.</returns>
        ISourceProvider GetSourceProvider();

        /// <summary>
        /// Get the default page settings for this project type. 
        /// </summary>
        /// <returns>The default page settings for this project type.</returns>
        SettingsCollection GetDefaultPageSettings();

        /// <summary>
        /// Get the default project settings for this project type.
        /// </summary>
        /// <returns>The default project settings for this project type.</returns>
        SettingsCollection GetDefaultProjectSettings();

        /// <summary>
        /// Get the default section settings for this project type. 
        /// </summary>
        /// <returns>The default section settings for this project type.</returns>
        SettingsCollection GetDefaultSectionSettings();

        /// <summary>
        /// Get the default control settings for this project type. 
        /// </summary>
        /// <returns>The default control settings for this project type.</returns>
        SettingsCollection GetDefaultControlSettings();

        /// <summary>
        /// The studio environment.
        /// </summary>
        IStudioEnvironment Studio { get; set; }

        /// <summary>
        /// This method is called by the Studio as soon as the <see cref="Studio"/> property is available.
        /// Use this method for initialization.
        /// </summary>
        void TypeProviderConnected();

        /// <summary>
        /// Get a node selector for selecting a section node.
        /// </summary>
        /// <returns>A node selector for selecting a section node.</returns>
        ISectionIdentifierSelector GetSectionSelector();

        /// <summary>
        /// Get a node selector for manually selecting a control node.
        /// </summary>
        /// <returns>A node selector for manually selecting a control node.</returns>
        IControlIdentifierSelector GetControlSelector();

        /// <summary>
        /// Get a control finder to automatically discover potential controls for a section or page.
        /// </summary>
        /// <returns>A control finder to automatically discover potential controls for a section or page.</returns>
        IControlFinder GetControlFinder();

        /// <summary>
        /// Get the bounds of the given control.
        /// </summary>
        /// <param name="control">The control to refresh.</param>
        /// <param name="section">The section the control is part of.</param>
        /// <param name="page">The page the section of the control is part of.</param>
        /// <param name="project">The parent project.</param>
        /// /// <returns>The control with it's <see cref="Control.ElementBounds"/> property filled with the bounds of the control and a bool indicating whether refreshing suceeded.</returns>
        (Control control, bool succes) RefreshControl(Control control, Section section, Page page, Project project);

        /// <summary>
        /// Get the bounds of the given section.
        /// </summary>
        /// <param name="section">The section to refresh.</param>
        /// <param name="page">The page the section is part of.</param>
        /// <param name="project">The parent project.</param>
        /// <returns>The section with it's <see cref="Section.ElementBounds"/> property filled with the bounds of the section and a bool indicating whether refreshing suceeded.</returns>
        (Section section, bool succes) RefreshSection(Section section, Page page, Project project);

        /// <summary>
        /// Get a screenshot for the given page.
        /// </summary>
        /// <param name="page">The page to make a screenshot for.</param>
        /// <param name="project">The parent project.</param>
        /// <returns>The page with it's <see cref="Page.Screenshot"/> property filled with data and a bool indicating whether refreshing suceeded.</returns>
        (Page page, bool succes) RefreshPage(Page page, Project project);
    }
}
