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

#if ASPNETCORE50
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
#else
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
#endif

                    await _next(context);

#if ASPNETCORE50
                    CultureInfo.CurrentCulture = originalCulture;
                    CultureInfo.CurrentUICulture = originalUICulture;
#else
                    Thread.CurrentThread.CurrentCulture = originalCulture;
                    Thread.CurrentThread.CurrentUICulture = originalUICulture;
#endif

                    return;
                }
            }
        }
    }
}