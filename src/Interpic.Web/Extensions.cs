using Interpic.Models;

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
