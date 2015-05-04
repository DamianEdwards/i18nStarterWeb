using System;
using System.Reflection;
using System.Resources;
using Microsoft.Framework.Runtime;

namespace Microsoft.Framework.Localization
{
    public class ResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IApplicationEnvironment _appEnv;

        public ResourceManagerStringLocalizerFactory(IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var baseName = typeInfo.FullName;
            return new ResourceManagerStringLocalizer(new ResourceManager(resourceSource), assembly, baseName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var assembly = Assembly.Load(new AssemblyName(location ?? _appEnv.ApplicationName));

            return new ResourceManagerStringLocalizer(new ResourceManager(baseName, assembly), assembly, baseName);
        }
    }
}