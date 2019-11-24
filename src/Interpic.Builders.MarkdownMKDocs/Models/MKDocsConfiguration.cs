using System.Collections.Generic;

namespace Interpic.Builders.MarkdownMKDocs.Models
{
    public class MKDocsConfiguration
    {
        public string site_name { get; set; }
        public string site_url { get; set; }
        public string repo_url { get; set; }
        public string repo_name { get; set; }
        public string edit_uri { get; set; }
        public string site_description { get; set; }
        public string site_author { get; set; }
        public string copyright { get; set; }
        public string google_analytics { get; set; }
        public string remote_branch { get; set; }
        public string remote_name { get; set; }
        public KeyValuePair<string, string>[] nav { get; set; }
    }
}
