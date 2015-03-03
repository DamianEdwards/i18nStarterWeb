using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Framework.Localization
{
    public interface IStringLocalizer : IEnumerable<LocalizedString>
    {
        LocalizedString this[string name] { get; }

        LocalizedString this[string name, params object[] arguments] { get; }

        LocalizedString GetString(string name);

        LocalizedString GetString(string name, params object[] values);

        IStringLocalizer WithCulture(CultureInfo culture);
    }
}