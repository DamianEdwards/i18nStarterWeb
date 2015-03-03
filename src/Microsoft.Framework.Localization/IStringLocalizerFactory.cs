using System;
using System.Reflection;

namespace Microsoft.Framework.Localization
{
    public interface IStringLocalizerFactory
    {
        IStringLocalizer Create(Type resourceSource);

        IStringLocalizer Create(string baseName, string location);
    }
}