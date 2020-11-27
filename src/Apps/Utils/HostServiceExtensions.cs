using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO;
using System.Threading;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Utils
{
    public static class HostServiceExtensions
    {
        /// <summary>
        /// Handy extension to get a scoped service.
        /// </summary>
        /// <typeparam name="T">Service</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns></returns>
        public static T GetRequiredService<T>(this IServiceCollection services) where T : class
        {
            var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    return scope.ServiceProvider.GetRequiredService<T>();
                }
                catch
                {
                    return default;
                }
            }
        }

        public static void AddStrings(this IConfigurationBuilder builder,
            string contentRoot)
        {
            var defaultCulture = new CultureInfo("en-US");            

            var localized = new FileInfo(Path.Combine(contentRoot, string.Format(Constants.StrPathPattern,
                CultureInfo.CurrentCulture.Name)));

            builder.AddJsonFile(string.Format(Constants.StrPathPattern, (localized.Exists ?
                CultureInfo.CurrentCulture : defaultCulture).Name))
                       .Build();
        }
    }
}
