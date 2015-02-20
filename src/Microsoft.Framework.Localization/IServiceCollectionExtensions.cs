using System;
using Microsoft.Framework.Localization;

namespace Microsoft.Framework.DependencyInjection
{
    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalization(this IServiceCollection services)
        {
            services.AddSingleton<ILocalizerFactory, ResourceManagerLocalizerFactory>();
            services.AddTransient(typeof(ILocalizer<>), typeof(Localizer<>));
            return services;
        }
    }
}