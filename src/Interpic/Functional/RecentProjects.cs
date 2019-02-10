using Interpic.Alerts;
using Interpic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Functional
{
    public static class RecentProjects
    {
        public static List<RecentProject> GetRecentProjects()
        {
            if (File.Exists(App.EXECUTABLE_DIRECTORY + "\\" + App.RECENT_PROJECTS_FILE))
            {
                try
                {
                    List<RecentProject> recentProjects = JsonConvert.DeserializeObject<List<RecentProject>>(File.ReadAllText(App.EXECUTABLE_DIRECTORY + "\\" + App.RECENT_PROJECTS_FILE));
                    bool changed = false;
                    List<RecentProject> remove = new List<RecentProject>();
                    foreach (RecentProject project in recentProjects)
                    {
                        if (!File.Exists(project.Path) || recentProjects.Count(recent => recent.Path == project.Path) > 1)
                        {
                            if (!remove.Any(recent => recent.Path == project.Path))
                            {
                                remove.Add(project);
                                changed = true;
                            }
                        }
                    }
                    if (changed)
                    {
                        foreach (RecentProject project in remove)
                        {
                            recentProjects.Remove(project);
                        }
                        WriteRecentProjects(recentProjects);
                    }
                    return recentProjects;

                }
                catch (Exception ex)
                {
                    ErrorAlert.Show("Could not load recent projects:\n" + ex.Message);
                    return new List<RecentProject>();
                }
            }
            else
            {
                return new List<RecentProject>();
            }
        }

        public static void AddToRecents(string name, string path)
        {
            List<RecentProject> recents = new List<RecentProject>();
            if (File.Exists(App.EXECUTABLE_DIRECTORY + "\\" + App.RECENT_PROJECTS_FILE))
            {
                recents.AddRange(GetRecentProjects());
            }
            if (!recents.Any(recent => recent.Path == path))
            {
                RecentProject newRecentProject = new RecentProject();
                newRecentProject.Name = name;
                newRecentProject.Path = path;
                recents.Add(newRecentProject);

                try
                {
                    File.WriteAllText(App.EXECUTABLE_DIRECTORY + "\\" + App.RECENT_PROJECTS_FILE, JsonConvert.SerializeObject(recents));
                }
                catch (Exception ex)
                {
                    ErrorAlert.Show("Could not save recent projects:\n" + ex.Message);
                }
            }
        }

        public static void WriteRecentProjects(List<RecentProject> projects)
        {
            try
            {
                File.WriteAllText(App.EXECUTABLE_DIRECTORY + "\\" + App.RECENT_PROJECTS_FILE, JsonConvert.SerializeObject(projects));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not save recent projects:\n" + ex.Message);
            }
        }

    }
}
