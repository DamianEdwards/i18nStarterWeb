using System;
using System.Collections.Generic;
using System.Globalization;
#if DNXCORE50
using System.Reflection;
#endif
using System.Resources;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerWithCultureStringLocalizer : ResourceManagerStringLocalizer
    {
        private readonly CultureInfo _culture;

#if DNX451
        public ResourceManagerWithCultureStringLocalizer(ResourceManager resourceManager, CultureInfo culture)
            : base(resourceManager)
#else
        public ResourceManagerWithCultureStringLocalizer(
            ResourceManager resourceManager,
            Assembly assembly,
            string baseName,
            CultureInfo culture)
            : base(resourceManager, assembly, baseName)
#endif
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