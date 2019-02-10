using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs
{
    public static class Settings
    {
        public const string GENERATE_CONFIGURATION = "generateConfiguration";
        public const string IMAGE_GENERATION_MODE = "imageGenerationMode";
        public const string CONFIGURATION_SETTINGS = "mkdocsConfigurationSettings";

        public static class ConfigurationSettings
        {
            public const string SITE_NAME = "siteName";
            public const string SITE_DESCRIPTION = "siteDescription";
            public const string SITE_AUTHOR = "siteAuthor";
            public const string SITE_URL = "siteUrl";
            public const string COPYRIGHT = "copyright";
            public const string DOCS_DIRECTORY = "docsDirectory";
            public const string SITE_DIRECTORY = "siteDirectory";
            public const string REPOSITORY_URL = "repositoryUrl";
            public const string REPOSITORY_NAME = "repositoryName";
            public const string EDIT_URL = "editUrl";
            public const string REMOTE_BRANCH = "remoteBranch";
            public const string REMOTE_NAME = "remoteName";
            public const string GOOGLE_ANALYTICS = "googleAnalytics";
            public const string NAVIGATION_SETTINGS = "navigationSettings";

            public static class NavigationSettings
            {
                public const string FOLDER_SETTING_START = "folderSetting";
            }

        }
    }
}
