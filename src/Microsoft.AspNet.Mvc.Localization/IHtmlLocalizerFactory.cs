using System;

namespace Microsoft.AspNet.Mvc.Localization
{
    public interface IHtmlLocalizerFactory
    {
        IHtmlLocalizer Create(Type resourceSource);
        IHtmlLocalizer Create(string baseName, string location);
    }
}