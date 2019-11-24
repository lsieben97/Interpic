namespace Interpic.Web
{
    public class RegularExpressions
    {
        public static readonly string STRIP_JAVASCRIPT = @"<script(.+?)*</script>";
        public static readonly string STRIP_CSS = @"<style(.+?)*</style>";
        public static readonly string STRIP_HTML_ENTITIES = "(&[a-z]{2,5};)";
    }
}
