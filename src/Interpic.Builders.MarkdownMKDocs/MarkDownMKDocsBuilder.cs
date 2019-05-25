using Interpic.AsyncTasks;
using Interpic.Builders.MarkdownMKDocs.Tasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs
{
    public class MarkDownMKDocsBuilder : IProjectBuilder
    {
        public IStudioEnvironment Studio { get; set; }

        public bool Build(BuildOptions options, Project project, Interpic.Models.Version version)
        {
            List<AsyncTasks.AsyncTask> tasksToExecute = new List<AsyncTasks.AsyncTask>();

            if (options.CleanOutputDirectory)
            {
                tasksToExecute.Add(new GenerateFolderStructureTask(options, project, version));
            }

            if (options.BuildSettings.GetBooleanSetting("generateConfiguration"))
            {
                tasksToExecute.Add(new GenerateMKDocsConfigurationTask(options, project, version));
            }

            int counter = 1;
            foreach (Page page in version.Pages)
            {
                tasksToExecute.Add(new GeneratePageTask(options, project, page, version, counter));
                counter++;
            }

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasksToExecute);
            dialog.ShowDialog();

            return !dialog.AllTasksCanceled;
        }

        public bool CleanOutputDirectory(Project project)
        {
            ProcessTaskDialog dialog = new ProcessTaskDialog(new CleanOutputDirectoryTask(project));
            dialog.ShowDialog();
            return !dialog.TaskToExecute.IsCanceled;
        }

        public string GetBuilderDescription()
        {
            return "Build markdown files ready for use with MKDocs";
        }

        public string GetBuilderId()
        {
            return "markdownMKDocs";
        }

        public string GetBuilderName()
        {
            return "Markdown (MKDocs)";
        }

        public SettingsCollection GetBuildSettings(Project project)
        {
            SettingsCollection settings = new SettingsCollection
            {
                Name = "build settings",
                BooleanSettings = new List<Setting<bool>>()
                {
                    new Setting<bool>
                    {
                        Name = "Generate mkdocs.yml",
                        Value = true,
                        Description = "Whether to generate a mkdocs.yml file at the root of the output folder.",
                        Key = Settings.GENERATE_CONFIGURATION
                    }
                },
                MultipleChoiceSettings = new List<MultipleChoiceSetting>()
                {
                    new MultipleChoiceSetting
                    {
                        Name = "Image generation mode",
                        Value = "base64",
                        Description = "The way Interpic saves images to the output folder.",
                        Key = Settings.IMAGE_GENERATION_MODE,
                        Choices = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Base64", "base64"),
                            new KeyValuePair<string, string>("Seperate files", "seperateFiles")
                        }
                    }
                },
                SubSettings = new List<Setting<SettingsCollection>>
                {
                    new Setting<SettingsCollection>
                    {
                        Name = "mkdocs.yml settings",
                        Description = "Settings for mkdocs.yml generation. Only used if Generate mkdocs.yml setting is set to yes.",
                        Key = "mkdocsConfigurationSettings",
                        Value = new SettingsCollection
                        {
                            Name = "mkdocs.yml settings",
                            TextSettings = new List<Setting<string>>
                            {
                                new Setting<string>
                                {
                                    Name = "Site name",
                                    Value = project.Name,
                                    Description = "The site_name setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.SITE_NAME

                                },
                                new Setting<string>
                                {
                                    Name = "Site description",
                                    Value = "",
                                    Description = "The site_description setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.SITE_DESCRIPTION
                                },
                                new Setting<string>
                                {
                                    Name = "Site author",
                                    Value = "",
                                    Description = "The site_author setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.SITE_AUTHOR
                                },
                                new Setting<string>
                                {
                                    Name = "Site url",
                                    Value = "",
                                    Description = "The site_url setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.SITE_URL
                                },
                                new Setting<string>()
                                {
                                    Name = "Copyright",
                                    Value = "",
                                    Description = "The copyright setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.COPYRIGHT
                                },
                                new Setting<string>()
                                {
                                    Name = "Docs dirictory",
                                    Value = "docs",
                                    Description = "The docs_dir setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.DOCS_DIRECTORY
                                },
                                new Setting<string>()
                                {
                                    Name = "Site dirictory",
                                    Value = "site",
                                    Description = "The site_dir setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.SITE_DIRECTORY
                                },
                                new Setting<string>
                                {
                                    Name = "Repository url",
                                    Value = "",
                                    Description = "The repo_url setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.REPOSITORY_URL
                                },
                                new Setting<string>
                                {
                                    Name = "Repository name",
                                    Value = "",
                                    Description = "The repo_name setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.REPOSITORY_NAME
                                },
                                new Setting<string>
                                {
                                    Name = "Edit url",
                                    Value = "",
                                    Description = "The edit_uri setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.EDIT_URL
                                },
                                new Setting<string>
                                {
                                    Name = "Remote branch",
                                    Value = "",
                                    Description = "The remote_branch setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.REMOTE_BRANCH
                                },
                                new Setting<string>
                                {
                                    Name = "Remote name",
                                    Value = "",
                                    Description = "The remote_name setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.REMOTE_NAME
                                },
                                new Setting<string>
                                {
                                    Name = "Google analytics",
                                    Value = "",
                                    Description = "The google_analytics setting in mkdocs.yml",
                                    Key = Settings.ConfigurationSettings.GOOGLE_ANALYTICS
                                }
                            },
                            SubSettings = new List<Setting<SettingsCollection>>()
                            {
                                new Setting<SettingsCollection>
                                {
                                    Key = Settings.ConfigurationSettings.NAVIGATION_SETTINGS,
                                    Name = "Navigation settings",
                                    Description = "Navigation settings per page",
                                    Value = new SettingsCollection()
                                    {
                                        Name = "Navigation settings"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return settings;
        }

        public List<string> GetCompatibleProjectTypes()
        {
            return null;
        }
    }
}
