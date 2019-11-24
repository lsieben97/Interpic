using Interpic.Alerts;
using Interpic.Models;
using Interpic.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs.Tasks
{
    class GeneratePageTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public Page Page { get; set; }
        public Interpic.Models.Version Version { get; set; }
        public BuildOptions Options { get; set; }
        public GeneratePageTask(BuildOptions options, Project project, Page page, Interpic.Models.Version version, int pageNumber)
        {
            TaskName = "Generating page " + pageNumber.ToString() + "...";
            TaskDescription = page.Name;
            ActionName = "Generate page " + pageNumber.ToString();
            IsIndeterminate = true;
            Project = project;
            Options = options;
            Page = page;
            Version = version;
        }
        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            Image screenshot = Image.FromStream(new MemoryStream(Page.Screenshot));
            StringBuilder page = new StringBuilder();
            page.Append("# " + Page.Name);
            page.AppendLine();
            page.AppendLine();
            page.Append(Page.Description);

            page.AppendLine();
            page.AppendLine();

            foreach(Section section in Page.Sections)
            {
                page.Append("## " + section.Name);

                page.AppendLine();
                page.AppendLine();

                Image croppedImage = ImageUtils.CropImage(screenshot, section.ElementBounds);
                page.Append("<img src=\"data:image/png;base64," + ImageUtils.ImageToBase64(croppedImage, System.Drawing.Imaging.ImageFormat.Png) + "\" usemap=\"#section" + section.Name.Replace(" ", "") + "\" />");
                StringBuilder imageMap = new StringBuilder();
                imageMap.Append("<map name=\"section" + section.Name.Replace(" ", "") + "\">");
                imageMap.AppendLine();
                foreach (Control control in section.Controls)
                {
                    int topLeftX = control.ElementBounds.Location.X - section.ElementBounds.Location.X;
                    int topLeftY = control.ElementBounds.Location.Y - section.ElementBounds.Location.Y;
                    int bottomRightX = control.ElementBounds.Location.X + control.ElementBounds.Size.Width - section.ElementBounds.Location.X;
                    int bottomRightY = control.ElementBounds.Location.Y + control.ElementBounds.Size.Height - section.ElementBounds.Location.Y;
                    string hrefTarget = control.Name.Replace(" ", "-").ToLower();
                    imageMap.Append("\t<area target=\"_self\" alt=\"" + control.Name + "\" href=\"#" + hrefTarget + "\" title=\"" + control.Name + "\" coords=\"" + topLeftX.ToString() + "," + topLeftY.ToString() + "," + bottomRightX.ToString() + "," + bottomRightY.ToString() + "\" shape=\"rect\">");
                    imageMap.AppendLine();
                }
                imageMap.AppendLine();
                imageMap.Append("</map>");

                page.AppendLine();
                page.Append(imageMap.ToString());

                page.AppendLine();
                page.AppendLine();
                page.Append(section.Description);
                page.AppendLine();
                page.AppendLine();

                foreach (Control control in section.Controls)
                {
                    page.Append("### " + control.Name);

                    page.AppendLine();
                    page.AppendLine();

                    Image croppedControlImage = ImageUtils.CropImage(screenshot, control.ElementBounds);
                    page.Append("<img src=\"data:image/png;base64," + ImageUtils.ImageToBase64(croppedControlImage, System.Drawing.Imaging.ImageFormat.Png) + "\" />");
                    page.AppendLine();
                    page.AppendLine();
                    page.Append(control.Description);
                    page.AppendLine();
                    page.AppendLine();
                }
            }

            try
            {
                string docsFolder = Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.DOCS_DIRECTORY);
                File.WriteAllText(Project.OutputFolder + Path.DirectorySeparatorChar + Version.Name + Path.DirectorySeparatorChar + docsFolder + Path.DirectorySeparatorChar + Page.Name + ".md", page.ToString());
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not create page file.\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }
    }
}
