using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Framework.Localization;

namespace Microsoft.AspNet.Mvc.Localization
{
    public class HtmlLocalizer<TResourceSource> : IHtmlLocalizer<TResourceSource>
    {
        private readonly IHtmlLocalizer _localizer;

        public HtmlLocalizer(IHtmlLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(TResourceSource));
        }

        public virtual IHtmlLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        ILocalizer ILocalizer.WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        public virtual LocalizedString this[string key] => _localizer[key];

        public virtual LocalizedString this[string key, params object[] arguments] => _localizer[key, arguments];

        public virtual LocalizedString GetString(string key) => _localizer.GetString(key);

        public virtual LocalizedString GetString(string key, params object[] arguments) => _localizer.GetString(key, arguments);

        public virtual LocalizedHtmlString Html(string key) => _localizer.Html(key);

        public virtual LocalizedHtmlString Html(string key, params object[] arguments) => _localizer.Html(key, arguments);

        public IEnumerator<LocalizedString> GetEnumerator() => _localizer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _localizer.GetEnumerator();
    }
}