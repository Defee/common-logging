#if NETSTANDARD
using System.Configuration;

namespace Common.TempNetstandardUtils.Options
{
    public class TrustOptions
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Configuration.TrustSection class
        //     using default settings.
        public TrustOptions()
        {

        }

        //
        // Summary:
        //     Gets or sets the name of the security level under which the application will
        //     run.
        //
        // Returns:
        //     The name of the trust level. The default is "Full".
        [ConfigurationProperty("level", IsRequired = true, DefaultValue = "Full")]
        [StringValidator(MinLength = 1)]
        public string Level { get; set; }
        //
        // Summary:
        //     Specifies the URL of origin for an application.
        //
        // Returns:
        //     A well-formed HTTP URL or an empty string (""). The default is an empty string.
        [ConfigurationProperty("originUrl", DefaultValue = "")]
        public string OriginUrl { get; set; }
        //
        // Summary:
        //     Gets or set a value that indicates whether page requests are automatically restricted
        //     to the permissions that are configured in the trust policy file that is applied
        //     to the ASP.NET application.
        //
        // Returns:
        //     true if requests are automatically restricted to the permissions that are configured
        //     in the trust policy file; otherwise, false.
        [ConfigurationProperty("processRequestInApplicationTrust", DefaultValue = true)]
        public bool ProcessRequestInApplicationTrust { get; set; }
        //
        // Summary:
        //     Gets or set a value that indicates whether the legacy code access security is
        //     enabled.
        //
        // Returns:
        //     true if legacy code access security is enabled; otherwise, false. The default
        //     is false.
        [ConfigurationProperty("legacyCasModel", DefaultValue = false)]
        public bool LegacyCasModel { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the permission set.
        //
        // Returns:
        //     The name of the permission set.
        [ConfigurationProperty("permissionSetName", DefaultValue = "ASP.Net")]
        public string PermissionSetName { get; set; }
        //
        // Summary:
        //     Gets or sets the custom security-policy resolution type.
        //
        // Returns:
        //     The custom security-policy resolution type.
        [ConfigurationProperty("hostSecurityPolicyResolverType", DefaultValue = "")]
        public string HostSecurityPolicyResolverType { get; set; }
        
    }
}
#endif