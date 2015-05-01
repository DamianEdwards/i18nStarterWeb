using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<MissingManifestCacheKey, object> _missingManifestCache =
            new ConcurrentDictionary<MissingManifestCacheKey, object>();

        public ResourceManagerStringLocalizer(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }

        protected ResourceManager ResourceManager { get; }

        public virtual LocalizedString this[string name] => GetString(name);
        
        public virtual LocalizedString this[string name, params object[] arguments] => GetString(name, arguments);

        public virtual LocalizedString GetString(string name)
        {
            var value = GetStringSafely(name, null);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }

        public virtual LocalizedString GetString(string name, params object[] arguments)
        {
            var format = GetStringSafely(name, null);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null);
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return culture == null
                ? new ResourceManagerStringLocalizer(ResourceManager)
                : new ResourceManagerWithCultureStringLocalizer(ResourceManager, culture);
        }

        protected string GetStringSafely(string name, CultureInfo culture)
        {
            var cacheKey = new MissingManifestCacheKey(name, culture);
            if (_missingManifestCache.ContainsKey(cacheKey))
            {
                return null;
            }

            try
            {
                return culture == null ? ResourceManager.GetString(name) : ResourceManager.GetString(name, culture);
            }
            catch (MissingManifestResourceException)
            {
                _missingManifestCache.TryAdd(cacheKey, null);
                return null;
            }
        }

        public virtual IEnumerator<LocalizedString> GetEnumerator()
        {
            return GetEnumerator(CultureInfo.CurrentUICulture);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected IEnumerator<LocalizedString> GetEnumerator(CultureInfo culture)
        {
#if DNX451
            var resourceSet = ResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: true);
            var enumerator = resourceSet.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return new LocalizedString(enumerator.Key.ToString(), enumerator.Value.ToString());
            }
#else
            throw new NotSupportedException(".NET Core doesn't support resource enumeration yet: https://github.com/dotnet/corefx/issues/948");
#endif
        }

        private class MissingManifestCacheKey : IEquatable<MissingManifestCacheKey>
        {
            private readonly int _hashCode;

            public MissingManifestCacheKey(string name, CultureInfo culture)
            {
                Name = name;
                CultureInfo = culture;
                _hashCode = new { Name, CultureInfo }.GetHashCode();
            }

            public string Name { get; }

            public CultureInfo CultureInfo { get; }

            public bool Equals(MissingManifestCacheKey other)
            {
                return string.Equals(Name, other.Name, StringComparison.Ordinal)
                    && CultureInfo == other.CultureInfo;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MissingManifestCacheKey;

                if (other != null)
                {
                    return Equals(other);
                }

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}