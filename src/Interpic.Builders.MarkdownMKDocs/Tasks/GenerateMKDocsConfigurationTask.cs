using Interpic.Alerts;
using Interpic.Builders.MarkdownMKDocs.Models;
using Interpic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs.Tasks
{
    class GenerateMKDocsConfigurationTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public BuildOptions Options { get; set; }
        public Interpic.Models.Version Version { get; set; }
        public GenerateMKDocsConfigurationTask(BuildOptions options, Project project, Interpic.Models.Version version)
        {
            TaskName = "Generating MKDocs configuration file...";
            TaskDescription = project.OutputFolder + "mkdocs.yml";
            ActionName = "Generate MKDocs configuration";
            IsIndeterminate = true;
            Project = project;
            Options = options;
            Version = version;
        }
        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            MKDocsConfiguration configuration = new MKDocsConfiguration();
            configuration.copyright = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.COPYRIGHT);
            configuration.edit_uri = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.EDIT_URL);
            configuration.google_analytics = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.GOOGLE_ANALYTICS);
            configuration.remote_branch = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.REMOTE_BRANCH);
            configuration.remote_name = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.REMOTE_NAME);
            configuration.repo_name = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.REPOSITORY_NAME);
            configuration.repo_url = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.REPOSITORY_URL);
            configuration.site_author = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_AUTHOR);
            configuration.site_description = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_DESCRIPTION);
            configuration.site_name = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_NAME);
            configuration.site_url = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_URL);

            List<KeyValuePair<string, string>> navigationConfiguration = new List<KeyValuePair<string, string>>();
            
            foreach (Page page in Version.Pages)
            {
                string folder = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetSubSettings(Settings.ConfigurationSettings.NAVIGATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.NavigationSettings.FOLDER_SETTING_START + page.Name);
                if (folder == "\\")
                {
                    folder = "";
                }
                navigationConfiguration.Add(new KeyValuePair<string, string>(page.Name, folder + page.Name + ".md"));
            }
            configuration.nav = navigationConfiguration.ToArray();

            StringBuilder configurationFile = new StringBuilder();
            if(!string.IsNullOrWhiteSpace(configuration.copyright))
            {
                configurationFile.Append("copyright: ");
                configurationFile.Append("'" + configuration.copyright + "'");
                configurationFile.AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(configuration.edit_uri))
            {
                configurationFile.Append("edit_uri: ");
                configurationFile.Append("'" + configuration.edit_uri + "'");
                configurationFile.AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(configuration.google_analytics))
            {
                configurationFile.Append("google_analytics: ");
                configurationFile.Append("'" + configuration.google_analytics + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.remote_branch))
            {
                configurationFile.Append("remote_branch: ");
                configurationFile.Append("'" + configuration.remote_branch + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.remote_name))
            {
                configurationFile.Append("remote_name: ");
                configurationFile.Append("'" + configuration.remote_name + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.repo_name))
            {
                configurationFile.Append("repo_name: ");
                configurationFile.Append("'" + configuration.repo_name + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.repo_url))
            {
                configurationFile.Append("repo_url: ");
                configurationFile.Append("'" + configuration.repo_url + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.site_author))
            {
                configurationFile.Append("site_author: ");
                configurationFile.Append("'" + configuration.site_author + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.site_description))
            {
                configurationFile.Append("site_description: ");
                configurationFile.Append("'" + configuration.site_description + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.site_name))
            {
                configurationFile.Append("site_name: ");
                configurationFile.Append("'" + configuration.site_name + "'");
                configurationFile.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(configuration.site_url))
            {
                configurationFile.Append("site_url: ");
                configurationFile.Append("'" + configuration.site_url + "'");
                configurationFile.AppendLine();
            }

            configurationFile.Append("nav:");
            configurationFile.AppendLine();

            foreach(KeyValuePair<string, string> navigation in configuration.nav)
            {
                configurationFile.Append("  - '" + navigation.Key + "': '" + navigation.Value + "'");
                configurationFile.AppendLine();
            }

            try
            {
                File.WriteAllText(Project.OutputFolder + Path.DirectorySeparatorChar + Version.Name + Path.DirectorySeparatorChar + "mkdocs.yml", configurationFile.ToString());
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not create MKDocs configuration file.\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }
    }
}
