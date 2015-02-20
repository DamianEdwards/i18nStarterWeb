using System;
using Microsoft.AspNet.Localization;

namespace Microsoft.AspNet.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestLocalization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLocalizationMiddleware>();
        }
    }
}