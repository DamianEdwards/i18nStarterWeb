using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerWithCultureLocalizer : ResourceManagerLocalizer
    {
        private readonly CultureInfo _culture;

        public ResourceManagerWithCultureLocalizer(ResourceManager resourceManager, CultureInfo culture)
            : base(resourceManager)
        {
            _culture = culture;
        }

        public override LocalizedString this[string name] => GetString(name);

        public override LocalizedString this[string name, params object[] arguments] => GetString(name, arguments);

        public override LocalizedString GetString(string name)
        {
            var value = GetStringSafely(name, _culture);
            return new LocalizedString(name, value ?? name);
        }

        public override LocalizedString GetString(string name, params object[] arguments)
        {
            var format = GetStringSafely(name, _culture);
            var value = string.Format(_culture, format ?? name, arguments);
            return new LocalizedString(name, value ?? name, resourceNotFound: format == null);
        }

        public override IEnumerator<LocalizedString> GetEnumerator()
        {
            return GetEnumerator(_culture);
        }
    }
}