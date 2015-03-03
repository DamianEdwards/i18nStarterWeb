using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Localization;
using Microsoft.Framework.Runtime;

namespace InternationalizedStarterWeb.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IApplicationEnvironment _app;
        private readonly IFileProvider _files;

        public ResourcesController(IStringLocalizerFactory localizerFactory, IApplicationEnvironment app)
        {
            _localizerFactory = localizerFactory;
            _app = app;
            _files = new PhysicalFileProvider(_app.ApplicationBasePath + "/Resources/bin");
        }

        public IDictionary<string, string> Index([FromQuery] string baseName)
        {
            var localizer = _localizerFactory.Create(_app.ApplicationName + "." + baseName, _app.ApplicationName);
            return localizer.ToDictionary(ls => ls.Key, ls => ls.Value);
        }

        public IDictionary<string, string> ForView([FromQuery] string view)
        {
            var basePath = view.Replace("/", ".");
            if (basePath.StartsWith("."))
            {
                basePath = basePath.Substring(1);
            }
            return Index(basePath);
        }
    }
}
