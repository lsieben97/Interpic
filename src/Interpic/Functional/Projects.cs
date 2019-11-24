using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Tasks;
using Interpic.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Interpic.Studio.Functional
{
    public static class Projects
    {
        public static Project LoadProject(string path)
        {
            try
            {
                Project project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
                // when the project has been moved
                if (project.Path != path)
                {
                    project.Path = path;
                }

                if (!Directory.Exists(project.OutputFolder))
                {
                    WarningAlert.Show("Output folder not found.\nPlease specify an output folder.");
                    System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        project.OutputFolder = dialog.SelectedPath;
                        SaveProjectTask task = new SaveProjectTask(project);
                        ProcessTaskDialog saveDialog = new ProcessTaskDialog(task, "Saving...");
                        saveDialog.ShowDialog();
                        if (saveDialog.TaskToExecute.IsCanceled)
                        {
                            throw new Exception("Could not save the project.");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid path.");
                    }
                }


                if (!Directory.Exists(project.OutputFolder))
                {
                    WarningAlert.Show("Project folder not found.\nPlease specify an output folder.");
                    System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        project.ProjectFolder = dialog.SelectedPath;
                        SaveProjectTask task = new SaveProjectTask(project);
                        ProcessTaskDialog saveDialog = new ProcessTaskDialog(task, "Saving...");
                        saveDialog.ShowDialog();
                        if (saveDialog.TaskToExecute.IsCanceled)
                        {
                            throw new Exception("Could not save the project.");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid path.");
                    }
                }
                return JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not load project:\n" + ex.Message);
            }
            return null;
        }

        public static TreeViewItem GetTreeViewForProject(Project project, Models.Version currentVersion)
        {
            TreeViewItem root = UiUtils.GetTreeViewItem(project.Name, "ProjectWhite.png");
            root.Tag = project;
            project.TreeViewItem = root;
            foreach (Models.Page page in currentVersion.Pages)
            {
                TreeViewItem pageItem = UiUtils.GetTreeViewItem(page.Name, "PageWhite.png");
                pageItem.Tag = page;
                page.TreeViewItem = pageItem;
                foreach (Section section in page.Sections)
                {
                    TreeViewItem sectionItem = UiUtils.GetTreeViewItem(section.Name, "SectionWhite.png");
                    sectionItem.Tag = section;
                    section.TreeViewItem = sectionItem;
                    foreach (Models.Control control in section.Controls)
                    {
                        TreeViewItem controlItem = UiUtils.GetTreeViewItem(control.Name, "ControlWhite.png");
                        controlItem.Tag = control;
                        control.TreeViewItem = controlItem;
                        sectionItem.Items.Add(controlItem);
                    }
                    pageItem.Items.Add(sectionItem);
                }
                root.Items.Add(pageItem);
            }
            return root;
        }


        public static bool SaveProject(Project project)
        {
            try
            {
                File.WriteAllText(project.Path, JsonConvert.SerializeObject(project));
                project.LastSaved = DateTime.Now;
                project.Changed = false;
                return true;
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not save project:\n" + ex.Message);
            }
            return false;
        }

        public static bool SaveAsNewProject(Project project, string path)
        {
            //TODO: Copy source directory to target and save ipp file to target directory.
            string projectPath = path + Path.GetFileName(project.Path);
            string oldPath = new string(project.Path.ToCharArray());
            string oldProjectPath = new string(project.ProjectFolder.ToCharArray());
            string oldOutputFolder = new string(project.OutputFolder.ToCharArray());
            project.Path = projectPath;
            project.ProjectFolder = path;
            project.OutputFolder = path + "\\Output\\";
            project.LastSaved = DateTime.Now;
            project.Changed = false;
            try
            {
                File.WriteAllText(project.Path, JsonConvert.SerializeObject(project));
                project.Path = oldPath;
                project.ProjectFolder = oldProjectPath;
                project.OutputFolder = oldOutputFolder;
                return true;
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not save project:\n" + ex.Message);
            }
            project.Path = oldPath;
            project.ProjectFolder = oldProjectPath;
            project.OutputFolder = oldOutputFolder;
            return false;
        }

        public static bool SaveAsJson(Project project, string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(project));
                return true;
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not export project:\n" + ex.Message);
            }
            return false;
        }
    }
}
