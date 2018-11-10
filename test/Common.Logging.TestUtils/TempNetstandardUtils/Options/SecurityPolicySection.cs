#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime;
using System.Text;

namespace Common.TempNetstandardUtils.Options
{
    //
    // Summary:
    //     Defines configuration settings that are used to support the security infrastructure
    //     of a Web application. This class cannot be inherited.
    public sealed class SecurityPolicyOption
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Configuration.SecurityPolicySection
        //     class by using default settings.
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public SecurityPolicyOption()
        {

        }

        //
        // Summary:
        //     Gets the System.Web.Configuration.SecurityPolicySection.TrustLevels collection.
        //
        // Returns:
        //     A collection of System.Web.Configuration.SecurityPolicySection.TrustLevels objects.
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public IEnumerable<TrustLevel> TrustLevels { get; }
    }
}
#endif