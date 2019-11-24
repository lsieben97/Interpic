using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Utils
{
    public static class ObjectModelUtils
    {
        public static List<string> GetAllIds(Models.Version version)
        {
            List<string> result = new List<string>();
            result.Add(version.Id);
            foreach (Models.Page page in version.Pages)
            {
                result.AddRange(GetAllIds(page));
            }
            return result;
        }

        public static List<string> GetAllIds(Models.Page page)
        {
            List<string> result = new List<string>();
            result.Add(page.Id);
            foreach (Models.Section section in page.Sections)
            {
                result.AddRange(GetAllIds(section));
            }
            return result;
        }

        public static List<string> GetAllIds(Models.Section section)
        {
            List<string> result = new List<string>();
            result.Add(section.Id);
            foreach(Models.Control control in section.Controls)
            {
                result.Add(control.Id);
            }
            return result;
        }
    }
}
