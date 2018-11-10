#if NETSTANDARD
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Common.Utils.Extensions.Config.Json
{
    /// <summary>
    /// Extends <see cref="IConfigurationBuilder"/> to 
    /// </summary>
    public static class JsonStringConfigurationExtensions
    {
        private static readonly string StringProviderKey = "SourceString";

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds a JSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="stringProvider">string json-config to be added</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonFromString(this IConfigurationBuilder builder, string stringProvider)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return AddJsonFromString(builder, stringProvider: stringProvider, optional: false);
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds a JSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="stringProvider">string json-config to be added</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonFromString(this IConfigurationBuilder builder, string stringProvider, bool optional)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddJsonFromString(new JsonStringSource
            {
                SourceString = stringProvider,
                Optional = optional
            });
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds a JSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonFromString(this IConfigurationBuilder builder, JsonStringSource configureSource)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return builder.Add(configureSource);
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Sets the default <see cref="String"/> to be used for file-based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="stringProvider">The default string provider instance.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetStringProvider(this IConfigurationBuilder builder, string stringProvider)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Properties[StringProviderKey] = stringProvider ?? throw new ArgumentNullException(nameof(stringProvider));
            return builder;
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Gets the default <see cref="IFileProvider"/> to be used for file-based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static string GetStringProvider(this IConfigurationBuilder builder)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.Properties.TryGetValue(StringProviderKey, out object provider))
            {
                return builder.Properties[StringProviderKey] as string;
            }

            return null;
        }
    }
}
#endif