using System;
using System.Reflection;
using System.Resources;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ResourceManagerLocalizerOptions _options;
        private readonly IApplicationEnvironment _appEnv;

        public ResourceManagerStringLocalizerFactory(IOptions<ResourceManagerLocalizerOptions> options, IApplicationEnvironment appEnv)
        {
            _options = options.Options;
            _appEnv = appEnv;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new ResourceManagerStringLocalizer(new ResourceManager(resourceSource));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (!string.IsNullOrEmpty(_options.ResourceFilesDirectory))
            {
#if ASPNET50
                if (baseName.StartsWith(_appEnv.ApplicationName + "."))
                {
                    baseName = baseName.Substring(_appEnv.ApplicationName.Length + 1);
                }                

                return new ResourceManagerStringLocalizer(
                    ResourceManager.CreateFileBasedResourceManager(
                        baseName,
                        _options.ResourceFilesDirectory,
                        usingResourceSet: null));
#else
                throw new NotSupportedException(".NET Core doesn't support file based resources yet: https://github.com/dotnet/corefx/issues/947");
#endif
            }
            var assembly = Assembly.Load(new AssemblyName(location));
            return new ResourceManagerStringLocalizer(new ResourceManager(baseName, assembly));
        }
    }

    public class ResourceManagerLocalizerOptions
    {
        public string ResourceFilesDirectory { get; set; }
    }
}