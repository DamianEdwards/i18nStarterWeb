using System;
using System.Globalization;
using Microsoft.Framework.Localization;

namespace Microsoft.AspNet.Mvc.Localization
{
    public interface IHtmlLocalizer : ILocalizer
    {
        new IHtmlLocalizer WithCulture(CultureInfo culture);

        LocalizedHtmlString Html(string key);

        LocalizedHtmlString Html(string key, params object[] arguments);
    }
}