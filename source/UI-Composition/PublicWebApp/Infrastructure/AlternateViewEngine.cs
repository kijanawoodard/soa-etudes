using System.Web.Mvc;

namespace PublicWebApp.Infrastructure
{
    public class AlternateViewEngine : RazorViewEngine
    {
        public AlternateViewEngine()
        {
            ViewLocationFormats = new[] { "~/Features/{1}/{0}.cshtml" };
            PartialViewLocationFormats = new[] { "~/Features/{1}/{0}.cshtml" }; 
        }
    }
}