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

        public Task Invoke(HttpContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                var queryCulture = context.Request.Query["culture"];
                if (!string.IsNullOrEmpty(queryCulture))
                {
                    var culture = new CultureInfo(queryCulture);
#if ASPNETCORE50
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = culture;
#else
                    // TODO: This is completely wrong as the threads get re-used!!
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
#endif
                }
            }

            return _next(context);
        }
    }
}