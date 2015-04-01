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
            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var baseName = typeInfo.FullName;
#if DNX451
            return new ResourceManagerStringLocalizer(new ResourceManager(resourceSource));
#else
            return new ResourceManagerStringLocalizer(new ResourceManager(resourceSource), assembly, baseName);
#endif
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var assembly = Assembly.Load(new AssemblyName(location ?? _appEnv.ApplicationName));

#if DNX451
            return new ResourceManagerStringLocalizer(new ResourceManager(baseName, assembly));
#else
            return new ResourceManagerStringLocalizer(new ResourceManager(baseName, assembly), assembly, baseName);
#endif
            
//            if (!string.IsNullOrEmpty(_options.ResourceFilesDirectory))
//            {
//#if DNX451
//                if (baseName.StartsWith(_appEnv.ApplicationName + "."))
//                {
//                    baseName = baseName.Substring(_appEnv.ApplicationName.Length + 1);
//                }                

//                return new ResourceManagerStringLocalizer(
//                    ResourceManager.CreateFileBasedResourceManager(
//                        baseName,
//                        _options.ResourceFilesDirectory,
//                        usingResourceSet: null));
//#else
//                throw new NotSupportedException(".NET Core doesn't support file based resources yet: https://github.com/dotnet/corefx/issues/947");
//#endif
//            }
        }
    }

    public class ResourceManagerLocalizerOptions
    {
        public string ResourceFilesDirectory { get; set; }
    }
}