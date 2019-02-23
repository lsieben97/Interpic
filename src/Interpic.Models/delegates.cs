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

    public delegate void OnPageSettingsOpening(object sender, PageSettingsEventArgs e);
    public delegate void OnPageSettingsOpened(object sender, PageSettingsEventArgs e);

    public delegate void OnSectionSettingsOpening(object sender, SectionSettingsEventArgs e);
    public delegate void OnSectionSettingsOpened(object sender, SectionSettingsEventArgs e);

    public delegate void OnControlSettingsOpening(object sender, ControlSettingsEventArgs e);
    public delegate void OnControlSettingsOpened(object sender, ControlSettingsEventArgs e);

    public delegate void OnGlobalSettingsSaved(object sender, GlobalSettingsEventArgs e);
    public delegate void OnNewVersionAdded(object sender, VersionEventArgs e);
    public delegate void OnVersionRemoved(object sender, VersionEventArgs e);

}