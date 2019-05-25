using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web
{
    public static class Extensions
    {
        public static WebPageExtensions GetWebPageExtensions(this Page page)
        {
            if (page.Extensions != null)
            {
                return (WebPageExtensions)page.Extensions;
            }
            return null;
        }
    }
}
