using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Settings;
using Interpic.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interpic.Studio.Functional
{
    public static class Projects
    { 
        public static Project LoadProject(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not load project:\n" + ex.Message);
            }
            return null;
        }

        public static TreeViewItem GetTreeViewForProject(Project project)
        {
            TreeViewItem root = UiUtils.GetTreeViewItem(project.Name, "ProjectWhite.png");
            root.Tag = project;
            project.TreeViewItem = root;
            foreach (Models.Page page in project.Pages)
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
            catch(Exception ex)
            {
                ErrorAlert.Show("Could not save project:\n" + ex.Message);
            }
            return false;
        }
    }
}
