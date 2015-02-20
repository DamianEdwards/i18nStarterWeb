using System;
using System.Reflection;

namespace Microsoft.Framework.Localization
{
    public interface ILocalizerFactory
    {
        ILocalizer Create(Type resourceSource);

        ILocalizer Create(string baseName, string location);
    }
}