#if NETSTANDARD
using Microsoft.Extensions.Configuration;

namespace Common.Utils.Extensions.Config.Xml
{
    /// <summary>
    /// Json String configuration source. Implements <see cref="IConfigurationSource"/>
    /// </summary>
    public class XmlStringSource:IConfigurationSource
    {
        /// <summary>
        /// String configuration
        /// </summary>
        public string SourceString { get; set; }
        /// <summary>
        /// Determines if loading the file is optional.
        /// </summary>
        public bool Optional { get; set; }
#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
#pragma warning restore CS3002 // Return type is not CLS-compliant
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new XmlStringProvider(this);
        }
    }
}
#endif