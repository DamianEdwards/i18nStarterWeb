using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Localization;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Mvc.Localization
{
    public class ViewLocalizer : IViewLocalizer, ICanHasViewContext
    {
        private readonly IHtmlLocalizerFactory _localizerFactory;
        private readonly string _appName;
        private IHtmlLocalizer _localizer;

        public ViewLocalizer(IHtmlLocalizerFactory localizerFactory, IApplicationEnvironment appEnv)
        {
            _appName = appEnv.ApplicationName;
            _localizerFactory = localizerFactory;
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public LocalizedString GetString(string name) => _localizer.GetString(name);

        public LocalizedString GetString(string name, params object[] values) => _localizer.GetString(name, values);

        public LocalizedHtmlString Html(string key) => _localizer.Html(key);

        public LocalizedHtmlString Html(string key, params object[] arguments) => _localizer.Html(key, arguments);

        public IStringLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        public IEnumerator<LocalizedString> GetEnumerator() => _localizer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _localizer.GetEnumerator();

        IHtmlLocalizer IHtmlLocalizer.WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        public void Contextualize(ViewContext viewContext)
        {
            var baseName = viewContext.View.Path.Replace('/', '.').Replace('\\', '.');
            if (baseName.StartsWith("."))
            {
                baseName = baseName.Substring(1);
            }
            baseName = _appName + "." + baseName;
            _localizer = _localizerFactory.Create(baseName, _appName);
        }
    }
}