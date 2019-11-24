using Interpic.Models.EventArgs;

namespace Interpic.Models
{
    public delegate void OnProjectLoaded(object sender, ProjectLoadedEventArgs e);
    public delegate void OnProjectUnloaded(object sender, InterpicStudioEventArgs e);
    public delegate void OnProjectCreated(object sender, InterpicStudioEventArgs e);

    public delegate void OnStudioStartup(object sender, InterpicStudioEventArgs e);
    public delegate void OnStudioShutdown(object sender, InterpicStudioEventArgs e);

    public delegate void OnProjectSettingsOpening(object sender, ProjectSettingsEventArgs e);
    public delegate void OnProjectSettingsOpened(object sender, ProjectSettingsEventArgs e);

    public delegate void OnVersionSettingsOpening(object sender, VersionSettingsEventArgs e);
    public delegate void OnVersionSettingsOpened(object sender, VersionSettingsEventArgs e);

    public delegate void OnPageSettingsOpening(object sender, PageSettingsEventArgs e);
    public delegate void OnPageSettingsOpened(object sender, PageSettingsEventArgs e);

    public delegate void OnSectionSettingsOpening(object sender, SectionSettingsEventArgs e);
    public delegate void OnSectionSettingsOpened(object sender, SectionSettingsEventArgs e);

    public delegate void OnControlSettingsOpening(object sender, ControlSettingsEventArgs e);
    public delegate void OnControlSettingsOpened(object sender, ControlSettingsEventArgs e);

    public delegate void OnGlobalSettingsSaved(object sender, GlobalSettingsEventArgs e);

    public delegate void OnMenuItemClicked(object sender, ProjectStateEventArgs e);

    public delegate void OnVersionRemoved(object sender, VersionEventArgs e);
    public delegate void OnPageRemoved(object sender, PageEventArgs e);
    public delegate void OnSectionRemoved(object sender, SectionEventArgs e);
    public delegate void OnControlRemoved(object sender, ControlEventArgs e);

    public delegate void OnVersionAdded(object sender, VersionEventArgs e);
    public delegate void OnPageAdded(object sender, PageEventArgs e);
    public delegate void OnSectionAdded(object sender, SectionEventArgs e);
    public delegate void OnControlAdded(object sender, ControlEventArgs e);
}