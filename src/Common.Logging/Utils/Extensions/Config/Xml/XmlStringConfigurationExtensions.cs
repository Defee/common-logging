#if NETSTANDARD
using System;
using Common.Utils.Extensions.Config.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Common.Utils.Extensions.Config.Xml
{
    /// <summary>
    /// Xml configuration extensions class for the string xml config.
    /// </summary>
    public static class XmlStringConfigurationExtensions
    {
        /// <summary>
        /// What will be used as the provider key
        /// </summary>
        private static readonly string StringProviderKey = "SourceString";

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds xml string to the configuration
        /// </summary>
        /// <param name="builder">configuration builder which got extended</param>
        /// <param name="stringProvider">string configuration</param>
        /// <returns>returns <see cref="IConfigurationBuilder"/></returns>
        public static IConfigurationBuilder AddXmlFromString(this IConfigurationBuilder builder, string stringProvider)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return AddXmlFromString(builder, stringProvider: stringProvider, optional: false);
        }


#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds a Xml configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="stringProvider"> string to be parsed to config</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddXmlFromString(this IConfigurationBuilder builder, string stringProvider, bool optional)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddXmlFromString(new XmlStringSource
            {
                SourceString = stringProvider,
                Optional = optional
            });
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Adds a Xml configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddXmlFromString(this IConfigurationBuilder builder, XmlStringSource configureSource)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return builder.Add(configureSource);
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// Sets the default <see cref="String"/> to be used for string-based providers.
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