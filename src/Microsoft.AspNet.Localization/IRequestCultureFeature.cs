using System;
using System.Globalization;

namespace Microsoft.AspNet.Localization
{
    public interface IRequestCultureFeature
    {
        CultureInfo Culture { get; }
    }

    public class RequestCultureFeature : IRequestCultureFeature
    {
        public RequestCultureFeature(CultureInfo culture)
        {
            Culture = culture;
        }

        public CultureInfo Culture { get; }
    }
}