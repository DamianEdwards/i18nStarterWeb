using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Framework.Localization
{
    public class StringLocalizer<TResourceSource> : IStringLocalizer<TResourceSource>
    {
        private IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(TResourceSource));
        }

        public virtual IStringLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        public virtual LocalizedString this[string key] => _localizer[key];

        public virtual LocalizedString this[string key, params object[] arguments] => _localizer[key, arguments];

        public virtual LocalizedString GetString(string key) => _localizer.GetString(key);

        public virtual LocalizedString GetString(string key, params object[] arguments) =>
            _localizer.GetString(key, arguments);

        public IEnumerator<LocalizedString> GetEnumerator() => _localizer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _localizer.GetEnumerator();
    }
}