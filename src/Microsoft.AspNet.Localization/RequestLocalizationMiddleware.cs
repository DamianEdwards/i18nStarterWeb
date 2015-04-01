using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Localization
{
    public class RequestLocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // TODO: Make this read from Accept-Language header, cookie, app-provided delegate, etc.
            if (context.Request.QueryString.HasValue)
            {
                var queryCulture = context.Request.Query["culture"];
                if (!string.IsNullOrEmpty(queryCulture))
                {
                    var culture = new CultureInfo(queryCulture);

                    context.SetFeature<IRequestCultureFeature>(new RequestCultureFeature(culture));

                    var originalCulture = CultureInfo.CurrentCulture;
                    var originalUICulture = CultureInfo.CurrentUICulture;

                    SetCurrentCulture(culture, culture);

                    await _next(context);

                    return;
                }
            }
            else
            {
                // NOTE: The below doesn't seem to be needed anymore now that DNX is correctly managing culture across
                //       async calls but we'll need to verify properly.
                // Forcibly set thread to en-US as sometimes previous threads have wrong culture across async calls, 
                // see note above.
                //var defaultCulture = new CultureInfo("en-US");
                //SetCurrentCulture(defaultCulture, defaultCulture);
            }

            await _next(context);
        }

        private void SetCurrentCulture(CultureInfo culture, CultureInfo uiCulture)
        {
#if DNX451
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
#else
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
#endif
        }
    }
}