using System;
using Microsoft.Framework.Localization;
using Microsoft.Framework.WebEncoders;

namespace Microsoft.AspNet.Mvc.Localization
{
    public class HtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly IStringLocalizerFactory _factory;
        private readonly IHtmlEncoder _encoder;

        public HtmlLocalizerFactory(IStringLocalizerFactory localizerFactory, IHtmlEncoder encoder)
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