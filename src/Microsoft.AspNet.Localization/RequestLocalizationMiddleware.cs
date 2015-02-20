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

                    // NOTE: If we're on a different thread now the culture is not here but is still on the old thread
                    //       which could be used on another request. This will be fixed by the XRE.

                    SetCurrentCulture(originalCulture, originalUICulture);

                    return;
                }
            }
            else
            {
                // Forcibly set thread to en-US as sometimes previous threads have wrong culture across async calls, 
                // see note above.
                var defaultCulture = new CultureInfo("en-US");
                SetCurrentCulture(defaultCulture, defaultCulture);
            }

            await _next(context);
        }

        private void SetCurrentCulture(CultureInfo culture, CultureInfo uiCulture)
        {
#if ASPNETCORE50
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
#else
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
#endif
        }
    }
}