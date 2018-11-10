#if NETSTANDARD
using System.Configuration;

namespace Common.TempNetstandardUtils.Options
{
    public class TrustLevel
    {
        public TrustLevel()
        {


        }
        //
        // Summary:
        //     Creates an instance of the System.Web.Configuration.TrustLevel class that is
        //     initialized based on the provided values, which define the mapping of specific
        //     security levels to named policy files.
        //
        // Parameters:
        //   name:
        //     A named security level that is mapped to a policy file.
        //
        //   policyFile:
        //     The configuration file that contains security policy settings for the named security
        //     level.
        public TrustLevel(string name, string policyFile)
        {
        }

        //
        // Summary:
        //     Gets or sets a named security level that is mapped to a policy file.
        //
        // Returns:
        //     The System.Web.Configuration.TrustLevel.Name that is mapped to a policy file.
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "Full", IsKey = true)]
        [StringValidator(MinLength = 1)]
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the configuration file reference that contains the security policy
        //     settings for the named security level.
        //
        // Returns:
        //     The configuration file reference that contains the security policy settings for
        //     the associated security level.
        [ConfigurationProperty("policyFile", IsRequired = true, DefaultValue = "internal")]
        public string PolicyFile { get; set; }
    }
}

#endif
