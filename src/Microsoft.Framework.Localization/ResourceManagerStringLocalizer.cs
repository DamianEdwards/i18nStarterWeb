using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
#if DNXCORE50
using System.Reflection;
#endif
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
#else
        public ResourceManagerStringLocalizer(
            ResourceManager resourceManager,
            Assembly resourceAssembly,
            string baseName)
        {
            ResourceManager = resourceManager;
            ResourceAssembly = resourceAssembly;
            ResourceBaseName = baseName;
        }
#endif

        protected ResourceManager ResourceManager { get; }

#if !DNX451
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
#else
                ? new ResourceManagerStringLocalizer(ResourceManager, ResourceAssembly, ResourceBaseName)
                : new ResourceManagerWithCultureStringLocalizer(
                    ResourceManager,
                    ResourceAssembly,
                    ResourceBaseName,
                    culture);
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
            // TODO: I'm sure something here should be cached, probably the whole result
            var currentCulture = culture;
            var resourceNames = new HashSet<string>();

            while (true)
            {
                try
                {
#if DNX451
                    // NOTE: Once System.Resources.ReaderWriter replaces System.Resources.ResourceWriter we can
                    //       just use that everywhere, no need to use GetResourceSet on DNX451.
                    var resources = ResourceManager.GetResourceSet(
                        currentCulture, 
                        createIfNotExists: true,
                        tryParents: true);
#else
                    var resourceStreamName = ResourceBaseName;
                    if (!string.IsNullOrEmpty(currentCulture.Name))
                    {
                        resourceStreamName += "." + currentCulture.Name;
                    }
                    resourceStreamName += ".resources";
                    using (var cultureResourceStream = ResourceAssembly.GetManifestResourceStream(resourceStreamName))
                    using (var resources = new ResourceReader(cultureResourceStream))
                    {
#endif
                        foreach (DictionaryEntry entry in resources)
                        {
                            var resourceName = (string)entry.Key;
                        }
#if !DNX451
                    }
#endif
                }
                catch (MissingManifestResourceException) { }

                if (currentCulture == currentCulture.Parent)
                {
                    // currentCulture begat currentCulture, probably time to leave
                    break;
                }

                currentCulture = currentCulture.Parent;
            }

            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name, culture);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
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