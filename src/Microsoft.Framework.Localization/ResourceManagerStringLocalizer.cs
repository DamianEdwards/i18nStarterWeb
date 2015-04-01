using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<MissingManifestCacheKey, object> _missingManifestCache =
            new ConcurrentDictionary<MissingManifestCacheKey, object>();

#if DNX451
        public ResourceManagerStringLocalizer(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
            ResourceBaseName = resourceManager.BaseName;
        }
#elif DNXCORE50
        public ResourceManagerStringLocalizer(ResourceManager resourceManager, Assembly resourceAssembly, string baseName)
        {
            ResourceManager = resourceManager;
            ResourceAssembly = resourceAssembly;
            ResourceBaseName = baseName;
        }
#endif

        protected ResourceManager ResourceManager { get; }

#if DNXCORE50
        protected Assembly ResourceAssembly { get; }
#endif

        protected string ResourceBaseName { get; }

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
#if DNX451
                ? new ResourceManagerStringLocalizer(ResourceManager)
                : new ResourceManagerWithCultureStringLocalizer(ResourceManager, culture);
#elif DNXCORE50
                ? new ResourceManagerStringLocalizer(ResourceManager, ResourceAssembly, ResourceBaseName)
                : new ResourceManagerWithCultureStringLocalizer(ResourceManager, ResourceAssembly, ResourceBaseName, culture);
#endif
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
            ResourceSet resourceSet;
            try
            {
                resourceSet = ResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: true);
            }
            catch (MissingManifestResourceException)
            {
                yield break;
            }
            
            var enumerator = resourceSet.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return new LocalizedString(enumerator.Key.ToString(), enumerator.Value.ToString());
            }
#else
            // TODO: Cache this maybe, at least the stream look up
            var resourceStreamName = $"{ResourceBaseName}.resources";
            using (var invariantCultureResourceStream = ResourceAssembly.GetManifestResourceStream(resourceStreamName))
            using (var reader = new ResourceReader(invariantCultureResourceStream))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    var resourceName = (string)entry.Key;
                    var value = GetStringSafely(resourceName, culture);
                    yield return new LocalizedString(resourceName, value ?? resourceName, resourceNotFound: value == null);
                }
            }
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