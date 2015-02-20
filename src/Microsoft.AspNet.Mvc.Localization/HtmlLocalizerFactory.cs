using System;
using Microsoft.AspNet.WebUtilities.Encoders;
using Microsoft.Framework.Localization;

namespace Microsoft.AspNet.Mvc.Localization
{
    public class HtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly ILocalizerFactory _factory;
        private readonly IHtmlEncoder _encoder;

        public HtmlLocalizerFactory(ILocalizerFactory localizerFactory, IHtmlEncoder encoder)
        {
            _factory = localizerFactory;
            _encoder = encoder;
        }

        public virtual IHtmlLocalizer Create(Type thing)
        {
            return new HtmlLocalizer(_factory.Create(thing), _encoder);
        }

        public virtual IHtmlLocalizer Create(string baseName, string location)
        {
            var localizer = _factory.Create(baseName, location);
            return new HtmlLocalizer(localizer, _encoder);
        }
    }
}